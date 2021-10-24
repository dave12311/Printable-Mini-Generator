using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System;
using static PrintableMiniGenerator.FifthEToolsParser;

namespace PrintableMiniGenerator
{
    [ValueConversion(typeof(List<Monster>), typeof(List<string>))]
    public class MonsterToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return ((List<Monster>)value).ConvertAll(e => { return e.Name; });
            }
            else
            {
                return new List<string>();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
