using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace TestLdap
{
    public class MainWindowViewModels
    {
        public ICollectionView TestProgress { get; private set; }
        public List<ProgressInfo> _testProgress = new List<ProgressInfo>();

        
        public MainWindowViewModels()
        {
            TestProgress = CollectionViewSource.GetDefaultView(_testProgress);
        }

        public void Refresh()
        {
            TestProgress.Refresh();
        }
        
        public ProgressInfo AddToProgress(DateTime date,
            string title,
            string status,
            bool passed,
            string note)
        {
            var info = new ProgressInfo
            {
                Date = date,
                Title = title,
                Status = status,
                Passed = passed,
                Note = note
            };
            _testProgress.Add(info);
            Refresh();
            return info;
        }
    }
}
