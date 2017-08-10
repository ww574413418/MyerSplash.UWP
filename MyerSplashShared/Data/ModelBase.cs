using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace MyerSplash.Data
{
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged<T>(Expression<Func<T>> exp)
        {
            var memberExpr = exp.Body as MemberExpression;
            if (memberExpr == null)
                throw new ArgumentException("propertyExpression should represent access to a member");
            string memberName = memberExpr.Member.Name;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
