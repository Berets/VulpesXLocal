using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.General
{
    public class ABEFreeWindowViewModel : Base
    {
        private IABERepository _abeRepository;
        public ABEFreeWindowViewModel(IABERepository abeRepository)
        {
            _abeRepository = abeRepository;
        }

        public ObservableCollection<int> GetFreeIDS()
        {
            var list = _abeRepository.GetFreeIDList();

            if (list == null || list.Count == 0)
            {
                list = new ObservableCollection<int>();
                for (int i = 1; i <= 1000; i++)
                { list.Add(i); }
            }

            return list;
        }
    }
}
