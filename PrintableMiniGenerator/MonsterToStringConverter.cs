using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System;
using System.Collections.ObjectModel;

namespace PrintableMiniGenerator
{
    [ValueConversion(typeof(List<FifthEToolsParser.Monster>), typeof(List<string>))]
    public class MonsterToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((List<FifthEToolsParser.Monster>)value).ConvertAll(e => { return e.Name; });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
