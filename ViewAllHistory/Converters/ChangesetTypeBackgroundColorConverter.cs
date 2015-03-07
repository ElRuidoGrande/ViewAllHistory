using TFSExtensions.ViewAllHistory.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TFSExtensions.ViewAllHistory.Converters
{
    public class ChangesetTypeBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ResultModel model = value as ResultModel;
                        
            if (model == null)
            {
                return Brushes.Transparent;
            }

            string input = model.Note;

            if (IsMergeChangeset(input))
            {
                return Brushes.LightGray;
            }


            return Brushes.Transparent;
        }

        private bool IsMergeChangeset(string input)
        {
            return input.ToLower().Contains("merge");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
