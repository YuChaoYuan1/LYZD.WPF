using LYZD.DAL;
using LYZD.Utility.Log;
using LYZD.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace LYZD.ViewModel.Menu
{
    /// <summary>
    /// 目录配置视图
    /// </summary>
    public class MenuViewModel : ViewModelBase
    {
        public MenuViewModel()
        {
            LoadCollections();
            LoadMenus();
        }
        private AsyncObservableCollection<MenuConfigItem> menus = new AsyncObservableCollection<MenuConfigItem>();
        /// <summary>
        /// 目录集合
        /// </summary>
        public AsyncObservableCollection<MenuConfigItem> Menus
        {
            get
            { return menus; }
            set
            { SetPropertyValue(value, ref menus, "Menus"); }
        }

        #region 列的数据源
        private ObservableCollection<ImageItem> images = new ObservableCollection<ImageItem>();
        /// <summary>
        /// 图片列表
        /// </summary>
        public ObservableCollection<ImageItem> Images
        {
            get { return images; }
            set { SetPropertyValue(value, ref images, "Images"); }
        }

        private ObservableCollection<EnumCheckingEnable> enables = new ObservableCollection<EnumCheckingEnable>();

        public ObservableCollection<EnumCheckingEnable> Enables
        {
            get { return enables; }
            set { SetPropertyValue(value, ref enables, "Enables"); }
        }

        private ObservableCollection<EnumMainMenu> yesNoCollection = new ObservableCollection<EnumMainMenu>();

        public ObservableCollection<EnumMainMenu> YesNoCollecion
        {
            get { return yesNoCollection; }
            set { SetPropertyValue(value, ref yesNoCollection, "YesNoCollecion"); }
        }

        private ObservableCollection<EnumMenuCategory> categories = new ObservableCollection<EnumMenuCategory>();

        public ObservableCollection<EnumMenuCategory> Categories
        {
            get { return categories; }
            set { SetPropertyValue(value, ref categories, "Categories"); }
        }
        private ObservableCollection<EnumMenuType> menuTypes = new ObservableCollection<EnumMenuType>();

        public ObservableCollection<EnumMenuType> MenuTypes
        {
            get { return menuTypes; }
            set { SetPropertyValue(value, ref menuTypes, "MenuTypes"); }
        }
        private ObservableCollection<EnumUserVisible> userTypes = new ObservableCollection<EnumUserVisible>();

        public ObservableCollection<EnumUserVisible> UserTypes
        {
            get { return userTypes; }
            set { SetPropertyValue(value, ref userTypes, "UserTypes"); }
        }
        #endregion

        /// <summary>
        /// 从数据库加载目录
        /// </summary>
        public void LoadMenus()
        {
            flagSort = false;

            Menus.Clear();
            List<DynamicModel> models = DALManager.ApplicationDbDal.GetList(EnumAppDbTable.T_MENU_VIEW.ToString());
            //models.GroupBy(item=>item.GetProperty(""))

            //IEnumerable< IGrouping<object, DynamicModel>> students = models.OrderBy(item => item.GetProperty("SORT_ID")).GroupBy(item => item.GetProperty("MENU_DATASOURCE"));

            ////IEnumerable<DynamicModel> modelsTemp = models.OrderBy(item => item.GetProperty("SORT_ID")).GroupBy(item => item.GetProperty(""));  //根据编号排序
            //foreach (var modelsTemp in students)
            //{
            //    foreach (DynamicModel modelTemp in modelsTemp)
            //    {
            //        MenuConfigItem itemTemp = new MenuConfigItem(modelTemp);
            //        itemTemp.Images = Images;
            //        string imageName = modelTemp.GetProperty("MENU_IMAGE") as string;  //获得图片名字
            //        itemTemp.MenuImage = Images.FirstOrDefault(item => item.ImageName == imageName);
            //        itemTemp.FlagChanged = false;
            //        Menus.Add(itemTemp);
            //        itemTemp.PropertyChanged += (sender, e) =>
            //        {
            //            if (flagSort && e.PropertyName == "SortId")
            //            {
            //                Menus.Sort(item => item.SortId);
            //            }
            //        };
            //    }
            //}


            IEnumerable<DynamicModel> modelsTemp = models.OrderBy(item => item.GetProperty("SORT_ID"));  //根据编号排序
            foreach (DynamicModel modelTemp in modelsTemp)
            {
                MenuConfigItem itemTemp = new MenuConfigItem(modelTemp);
                itemTemp.Images = Images;
                string imageName = modelTemp.GetProperty("MENU_IMAGE") as string;  //获得图片名字
                itemTemp.MenuImage = Images.FirstOrDefault(item => item.ImageName == imageName);
                itemTemp.FlagChanged = false;
                Menus.Add(itemTemp);
                itemTemp.PropertyChanged += (sender, e) =>
                {
                    if (flagSort && e.PropertyName == "SortId")
                    {
                        Menus.Sort(item => item.SortId);
                    }
                };
            }

            flagSort = true;
        }
        private void LoadCollections()
        {
            Images.Clear();
            string[] fileNames = Directory.GetFiles(string.Format(@"{0}\Images", Directory.GetCurrentDirectory()));
            for (int i = 0; i < fileNames.Length; i++)
            {
                try
                {
                    int indexTemp = fileNames[i].LastIndexOf('\\');
                    string fileName = fileNames[i].Substring(indexTemp + 1);
                    BitmapImage imageTemp = new BitmapImage(new Uri(fileNames[i], UriKind.Absolute));
                    ImageItem itemTemp = new ImageItem()
                    {
                        ImageName = fileName,
                        ImageControl = imageTemp
                    };
                    Images.Add(itemTemp);
                }
                catch
                { }
            }
            #region 加载集合
            Enables.Clear();
            Array arrayTemp = Enum.GetValues(typeof(EnumCheckingEnable));
            for (int i = 0; i < arrayTemp.Length; i++)
            {
                Enables.Add((EnumCheckingEnable)(arrayTemp.GetValue(i)));
            }
            YesNoCollecion.Clear();
            arrayTemp = Enum.GetValues(typeof(EnumMainMenu));
            for (int i = 0; i < arrayTemp.Length; i++)
            {
                YesNoCollecion.Add((EnumMainMenu)(arrayTemp.GetValue(i)));
            }
            Categories.Clear();
            arrayTemp = Enum.GetValues(typeof(EnumMenuCategory));
            for (int i = 0; i < arrayTemp.Length; i++)
            {
                Categories.Add((EnumMenuCategory)(arrayTemp.GetValue(i)));
            }
            MenuTypes.Clear();
            arrayTemp = Enum.GetValues(typeof(EnumMenuType));
            for (int i = 0; i < arrayTemp.Length; i++)
            {
                MenuTypes.Add((EnumMenuType)(arrayTemp.GetValue(i)));
            }
            UserTypes.Clear();
            arrayTemp = Enum.GetValues(typeof(EnumUserVisible));
            for (int i = 0; i < arrayTemp.Length; i++)
            {
                UserTypes.Add((EnumUserVisible)(arrayTemp.GetValue(i)));
            }
            #endregion
        }
        /// <summary>
        /// 保存目录到数据库
        /// </summary>
        public void SaveMenus()
        {
            for (int i = 0; i < Menus.Count; i++)
            {
                if (!Menus[i].FlagChanged)
                {
                    continue;
                }
                DynamicModel modelTemp = Menus[i].GetModel();
                string menuName = modelTemp.GetProperty("MENU_NAME") as string;
                int updateCount = DALManager.ApplicationDbDal.Update(EnumAppDbTable.T_MENU_VIEW.ToString(), 
                    string.Format("MENU_NAME='{0}'", menuName), Menus[i].GetModel(), new List<string>() { "MENU_IMAGE", "MENU_CLASS",
                    "VALID_FLAG", "MENU_DATASOURCE", "MENU_CHECK_ENABLE", "MENU_USER_VISIBLE", "MENU_MAIN", "MENU_CATEGORY", "SORT_ID" });
                if (updateCount == 1)
                {
                    LogManager.AddMessage(string.Format("更新目录:{0}的相关信息成功", menuName), EnumLogSource.数据库存取日志);
                    Menus[i].FlagChanged = false;
                }
                else
                {
                    LogManager.AddMessage(string.Format("更新目录:{0}的相关信息失败:没有执行更改", menuName), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                }
            }
        }

        private bool flagSort = false;
    }
}
