using System;
using System.Collections.Generic;

namespace Wheatech.ServiceModel.Autofac
{
    /// <summary>
    /// <see cref="EqualityComparer{T}" /> that uses a selector to get the key to compare objects.
    /// </summary>
    /// <typeparam name="TSource">The type of objects to compare.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    internal class SelectorEqualityComparer<TSource, TKey> : EqualityComparer<TSource>
    {
        /// <summary>
        /// The key selector.
        /// </summary>
        private readonly IEqualityComparer<TKey> _comparer;

        /// <summary>
        /// The key comparer.
        /// </summary>
        private readonly Func<TSource, TKey> _selector;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorEqualityComparer&lt;TSource, Tkey&gt;"/> class.
        /// </summary>
        /// <param name="selector">The key selector.</param>
        /// <param name="comparer">The key comparer.</param>
        public SelectorEqualityComparer(Func<TSource, TKey> selector, IEqualityComparer<TKey> comparer = null)
        {
            _selector = selector;
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <typeparamref name="TSource"/> to compare.</param>
        /// <param name="y">The second object of type <typeparamref name="TSource"/> to compare.</param>
        /// <returns><see langword="true" /> if the specified objects are equal; otherwise <see langword="false" />.</returns>
        public override bool Equals(TSource x, TSource y)
        {
            TKey xKey = _selector(x);
            TKey yKey = _selector(y);

            return _comparer.Equals(xKey, yKey);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object for which to get a hash code.</param>
        /// <returns>
        /// A hash code for the key of the specified object.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj"/> is a reference and <paramref name="obj"/> is <see langword="null" />.
        /// </exception>
        public override int GetHashCode(TSource obj)
        {
            TKey key = _selector(obj);

            return _comparer.GetHashCode(key);
        }
    }
}
