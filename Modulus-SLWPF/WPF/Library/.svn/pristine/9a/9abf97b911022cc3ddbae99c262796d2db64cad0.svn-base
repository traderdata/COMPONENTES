using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ModulusFE
{
    /// <summary>
    /// A small helper class that has a method to help create
    /// PropertyChangedEventArgs when using the INotifyPropertyChanged
    /// interface
    /// </summary>
    public static class ObservableHelper
    {
        #region Public Methods
        /// <summary>
        /// Creates PropertyChangedEventArgs
        /// </summary>
        /// <param name="propertyExpression">Expression to make 
        /// PropertyChangedEventArgs out of</param>
        /// <returns>PropertyChangedEventArgs</returns>
        public static PropertyChangedEventArgs CreateArgs<T>(Expression<Func<T, Object>> propertyExpression)
        {
            return new PropertyChangedEventArgs(GetPropName(propertyExpression));
        }

        ///<summary>
        ///</summary>
        ///<param name="propertyExpression"></param>
        ///<typeparam name="T"></typeparam>
        ///<returns></returns>
        public static string GetPropName<T>(Expression<Func<T, Object>> propertyExpression)
        {
            var lambda = propertyExpression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }
            var propertyInfo = memberExpression.Member;

            return propertyInfo.Name;
        }

        ///<summary>
        ///</summary>
        ///<param name="oldValue"></param>
        ///<param name="newValue"></param>
        ///<param name="eventChanged"></param>
        ///<param name="args"></param>
        ///<typeparam name="T"></typeparam>
        public static void NotifyIfDifferent<T>(ref T oldValue, T newValue,
          Action<PropertyChangedEventArgs> eventChanged, PropertyChangedEventArgs args)
        {
            if (Equals(oldValue, newValue))
                return;
            oldValue = newValue;
            eventChanged(args);
        }

        ///<summary>
        ///</summary>
        ///<param name="oldValue"></param>
        ///<param name="newValue"></param>
        ///<param name="eventChanged"></param>
        ///<typeparam name="T"></typeparam>
        public static void NotifyIfDifferent<T>(ref T oldValue, T newValue, Action eventChanged)
        {
            if (Equals(oldValue, newValue))
                return;
            oldValue = newValue;
            eventChanged();
        }
        #endregion
    }
}
