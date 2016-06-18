using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

namespace Wheatech.ServiceModel.Interception
{
    /// <summary>
    /// An attribute used to wraps the invocation with a <see cref="TransactionScope"/>
    /// </summary>
    public class TransactionScopeAttribute : InterceptorAttribute, IInterceptor
    {
        #region Fields

        private TimeSpan _scopeTimeout;
        private string _originalTimeout;
        private static readonly MethodInfo _genericWrapperMethod;

        #endregion

        #region Constructors

        static TransactionScopeAttribute()
        {
            _genericWrapperMethod = typeof(TransactionScopeAttribute).GetMethod("WrapTask", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeAttribute"/> class.
        /// </summary>
        public TransactionScopeAttribute()
            : this(TransactionScopeOption.Required)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeAttribute"/> class with the specified requirements.
        /// </summary>
        /// <param name="scopeOption">
        /// An instance of the <see cref="TransactionScopeOption"/> enumeration that describes the transaction requirements associated with this transaction scope.
        /// </param>
        public TransactionScopeAttribute(TransactionScopeOption scopeOption)
            : this(scopeOption, IsolationLevel.Unspecified)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeAttribute"/> class with the specified requirements.
        /// </summary>
        /// <param name="scopeOption">
        /// An instance of the <see cref="TransactionScopeOption"/> enumeration that describes the transaction requirements associated with this transaction scope.
        /// </param>
        /// <param name="isolationLevel">
        /// An instance of the <see cref="IsolationLevel"/> enumeration that describes the isolation level of the transaction.
        /// </param>
        public TransactionScopeAttribute(TransactionScopeOption scopeOption, IsolationLevel isolationLevel)
            : this(scopeOption, isolationLevel, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeAttribute"/> class with the specified requirements and COM+ interoperability requirements.
        /// </summary>
        /// <param name="scopeOption">
        /// An instance of the <see cref="TransactionScopeOption"/> enumeration that describes the transaction requirements associated with this transaction scope.
        /// </param>
        /// <param name="isolationLevel">
        /// An instance of the <see cref="IsolationLevel"/> enumeration that describes the isolation level of the transaction.
        /// </param>
        /// <param name="scopeTimeout">
        /// The <see cref="TimeSpan"/> after which the transaction scope times out and aborts the transaction.
        /// </param>
        public TransactionScopeAttribute(TransactionScopeOption scopeOption, IsolationLevel isolationLevel, string scopeTimeout)
        {
            ScopeOption = scopeOption;
            IsolationLevel = isolationLevel;
            Timeout = scopeTimeout;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the transaction scope option.
        /// </summary>
        /// <value>The transaction scope option.</value>
        public TransactionScopeOption ScopeOption { get; set; }

        /// <summary>
        /// Gets or sets the transaction isolation level.
        /// </summary>
        /// <value>The transaction isolation level.</value>
        public IsolationLevel IsolationLevel { get; set; }

        /// <summary>
        /// Gets or sets the transaction timeout.
        /// </summary>
        /// <value>The transaction timeout.</value>
        public string Timeout
        {
            get { return _originalTimeout; }
            set
            {
                _originalTimeout = value;
                ParseTimeout(_originalTimeout);
            }
        }

        #endregion

        #region Methods

        private void ParseTimeout(string scopeTimeout)
        {
            if (string.IsNullOrEmpty(scopeTimeout))
            {
                _scopeTimeout = TransactionManager.DefaultTimeout;
                return;
            }
            long ticks;
            if (long.TryParse(scopeTimeout, NumberStyles.Integer, CultureInfo.InvariantCulture, out ticks))
            {
                _scopeTimeout = TimeSpan.FromSeconds(long.Parse(scopeTimeout, CultureInfo.InvariantCulture));
            }
            else if (!TimeSpan.TryParse(scopeTimeout, CultureInfo.InvariantCulture, out _scopeTimeout))
            {
                throw new ArgumentException(string.Format("Cannot parse scope timeout '{0}'.", scopeTimeout),
                                            nameof(scopeTimeout));
            }
        }

        private TransactionOptions CreateTransactionOptions()
        {
            return new TransactionOptions
            {
                IsolationLevel = IsolationLevel,
                Timeout = _scopeTimeout
            };
        }

        /// <summary>
        /// Creates an interceptor as specified in the attribute configuration. 
        /// </summary>
        /// <param name="container">The <see cref="IServiceContainer"/> to use when creating interceptors, if necessary.</param>
        /// <returns>An interceptor object.</returns>
        public override IInterceptor CreateInterceptor(IServiceContainer container)
        {
            return this;
        }

        /// <summary>
        /// Execute interceptor processing.
        /// </summary>
        /// <param name="invocation">Inputs to the current call to the target.</param>
        /// <param name="getNext">Delegate to execute to get the next delegate in the interceptor chain.</param>
        /// <returns>Return value from the target.</returns>
        public IMethodReturn Invoke(IMethodInvocation invocation, GetNextInterceptorHandler getNext)
        {
            var method = (MethodInfo)invocation.Method;
            if (method.ReturnType == typeof(Task))
            {
                return invocation.CreateMethodReturn(WrapNoReturnTask(invocation, getNext));
            }
            if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var methodReturn = _genericWrapperMethod.MakeGenericMethod(method.ReturnType.GetGenericArguments()[0])
                    .Invoke(this, new object[] {invocation, getNext});
                return invocation.CreateMethodReturn(methodReturn);
            }
            using (var scope = new TransactionScope(ScopeOption, CreateTransactionOptions()))
            {
                var ret = getNext()(invocation, getNext);
                if (ret.Exception == null)
                {
                    scope.Complete();
                }
                return ret;
            }
        }

        private async Task WrapNoReturnTask(IMethodInvocation input, GetNextInterceptorHandler getNext)
        {
            using (var scope = new TransactionScope(ScopeOption, CreateTransactionOptions(), TransactionScopeAsyncFlowOption.Enabled))
            {
                await (Task)getNext()(input, getNext).ReturnValue;
                scope.Complete();
            }
        }

        // ReSharper disable once UnusedMember.Local
        private async Task<T> WrapTask<T>(IMethodInvocation input, GetNextInterceptorHandler getNext)
        {
            using (var scope = new TransactionScope(ScopeOption, CreateTransactionOptions(), TransactionScopeAsyncFlowOption.Enabled))
            {
                var value = await (Task<T>)getNext()(input, getNext).ReturnValue;
                scope.Complete();
                return value;
            }
        }

        #endregion
    }
}
