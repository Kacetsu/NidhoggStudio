namespace ns.Base {

    public interface ICloneable<T> {

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        T Clone();
    }
}