using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinJson.Controls;
using XamarinJson.Helpers;
using XamarinJson.Models;
using XamarinJson.Services;

namespace XamarinJson.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        //private ObservableCollection<Employee> _employeeList= new ObservableCollection<Employee>();
        private ObservableRangeCollection<Employee> _employeeList = new ObservableRangeCollection<Employee>();
        public ICommand LoginCommand { get; } //set을 안두는 이유는 생성자에서만 사용하기 때문에, 생성자 이외의 곳에서 사용할 경우 private set; 추가
        public ICommand SearchCommand { get; }
        public MainViewModel()
        {
            LoginCommand = new Command(async() => await Login());
            SearchCommand = new Command(async () => await Search());
        }

        private async Task Search()
        {
            string responseResult = string.Empty;
            string requestParamJson = string.Empty;

            Dictionary<string, string> requestDic = new Dictionary<string, string>();
            requestDic.Add("USP", "{? = call usp_get_emp_json(?)}"); //프로시저
            requestDic.Add("p_emp_nm", ""); //프로시저 파라미터와 동일하게

            responseResult = await BaseHttpService.Instance.SendRequestAsync(HttpCommand.GET, requestDic);
            //EmployeeList = new ObservableCollection<Employee>(JsonConvert.DeserializeObject<List<Employee>>(responseResult));

            EmployeeList.AddRange(JsonConvert.DeserializeObject<List<Employee>>(responseResult), System.Collections.Specialized.NotifyCollectionChangedAction.Reset);
        }

        private async Task Login()
        {
            Settings.AuthToken = await BaseHttpService.Instance.AuthorizationAsync("admin", "1234");

            Debug.WriteLine(Settings.AuthToken);
        }

        public ObservableRangeCollection<Employee> EmployeeList { get => _employeeList; set => SetProperty(ref this._employeeList, value); }
    }
}
