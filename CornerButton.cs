using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;

namespace demo.CustomControls
{
    public class CornerButton:Button
    {
        #region 属性
        public int BorderRadius

        {

            get { return (int)GetValue(BorderRadiusProperty); }

            set { SetValue(BorderRadiusProperty, value); }

        }

        public static readonly DependencyProperty BorderRadiusProperty = DependencyProperty.Register("BorderRadius", typeof(int), typeof(CornerButton), new FrameworkPropertyMetadata());

        private void SetValue(DependencyProperty borderRadiusProperty, int value)
        {
            throw new NotImplementedException();
        }

        private int GetValue(DependencyProperty borderRadiusProperty)
        {
            throw new NotImplementedException();
        }
        #endregion 属性

    }
}
