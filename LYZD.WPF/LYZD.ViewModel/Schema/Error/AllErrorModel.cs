using LYZD.Core.Enum;
using LYZD.DAL;
using LYZD.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LYZD.ViewModel.Schema.Error
{
    /// <summary>
    /// 所有检定点集合
    /// </summary>
    public class AllErrorModel : ViewModelBase
    {
        private AsyncObservableCollection<ErrorCategory> categories = new AsyncObservableCollection<ErrorCategory>();

        public AsyncObservableCollection<ErrorCategory> Categories
        {
            get { return categories; }
            set { categories = value; }
        }

        public void Load(List<DynamicModel> paraValues)
        {
            for (int i = 0; i < Categories.Count; i++)
            {
                Categories[i].PointsChanged -= category_PointsChanged;
                Categories[i].ErrorPoints.Clear();
            }
            Categories.Clear();

            //误差试验类型|功率方向|功率元件|功率因数|电流倍数|添加谐波|逆相序
            for (int i = 0; i < paraValues.Count; i++)
            {
                string stringPoint = paraValues[i].GetProperty("PARA_VALUE") as string;
                string[] arrayPara = stringPoint.Split('|');

                if (paraValues[i].GetProperty("PARA_NO") as string == ProjectID.初始固有误差)
                {
                    if (arrayPara.Length >= 4)
                    { 
                        ErrorCategory category = Categories.FirstOrDefault(item => item.Fangxiang == arrayPara[0] && item.Component == arrayPara[1]);
                        if (category == null)
                        {
                            category = new ErrorCategory
                            {
                                Fangxiang = arrayPara[0],
                                Component = arrayPara[1]
                            };
                            category.PointsChanged += category_PointsChanged;
                            Categories.Add(category);
                        }
                        ErrorModel errorPoint = category.ErrorPoints.FirstOrDefault(item => item.Current == arrayPara[3] && item.Factor == arrayPara[2]);
                        if (errorPoint == null)
                        {
                            category.ErrorPoints.Add(new ErrorModel
                            {
                                FangXiang = category.Fangxiang,
                                Component = category.Component,
                                Current = arrayPara[3],
                                Factor = arrayPara[2]
                            });
                        }
                    }
                }
                else
                {
                    if (arrayPara.Length >= 7)
                    {
                        ErrorCategory category = Categories.FirstOrDefault(item => item.Fangxiang == arrayPara[1] && item.Component == arrayPara[2] && item.ErrorType == arrayPara[0]);
                        if (category == null)
                        {
                            category = new ErrorCategory
                            {
                                ErrorType = arrayPara[0],
                                Fangxiang = arrayPara[1],
                                Component = arrayPara[2]
                            };
                            category.PointsChanged += category_PointsChanged;
                            Categories.Add(category);
                        }
                        //误差点不会重复,如果想重复
                        //把这个为空的判断取消掉
                        //但是界面上处理,取消检定点的时候只会删除一个点
                        ErrorModel errorPoint = category.ErrorPoints.FirstOrDefault(item => item.Current == arrayPara[4] && item.Factor == arrayPara[3]);
                        if (errorPoint == null)
                        {
                            category.ErrorPoints.Add(new ErrorModel
                            {
                                FangXiang = category.Fangxiang,
                                Component = category.Component,
                                ErrorType = category.ErrorType,
                                Current = arrayPara[4],
                                Factor = arrayPara[3]
                            });
                        }
                    }
                }


            }
            for (int i = 0; i < Categories.Count; i++)
            {
                Categories[i].FlagLoad = true;
            }
        }

        public void AddCategory()
        {
            ErrorCategory category = new ErrorCategory();
            //if (Categories.Any(item => item.Fangxiang == ""))
            //{

            //}//TODO 这里其实需要自动的判断如功率方向等，应该自动添加没有的项目  

            category.PointsChanged += category_PointsChanged;
            Categories.Add(category);
        }


        private string GetFangXian(int index)
        {
            string str= "";
            switch (index)
            {
                case 0:
                    str = "正向有功";
                    break;
                case 1:
                    str = "反向有功";
                    break;
                case 2:
                    str = "正向无功";
                    break;
                case 3:
                    str = "反向无功";
                    break;
                default:
                    break;
            }
            return str;
        }

        void category_PointsChanged(object sender, EventArgs e)
        {
            if (PointsChanged != null)
            {
                PointsChanged(sender, e);
            }
        }

        /// <summary>
        /// 检定点数量发生变化时,sender:变化的检定点.e:0:移除,1:添加
        /// </summary>
        public event EventHandler PointsChanged;
        private string lapCountIb = "2";
        /// <summary>
        /// 相对于Ib圈数
        /// </summary>
        public string LapCountIb
        {
            get { return lapCountIb; }
            set
            {
                if (lapCountIb != value)
                {
                    SetPropertyValue(value, ref lapCountIb, "LapCountIb");
                    for (int i = 0; i < Categories.Count; i++)
                    {
                        Categories[i].LapCountIb = LapCountIb;
                        for (int j = 0; j < Categories[i].ErrorPoints.Count; j++)
                        {
                            Categories[i].ErrorPoints[j].LapCountIb = LapCountIb;
                        }
                    }
                }
            }
        }
        private string guichengMulti = "100";
        /// <summary>
        /// 规程误差限倍数
        /// </summary>
        public string GuichengMulti
        {
            get { return guichengMulti; }
            set
            {
                if (guichengMulti != value)
                {
                    SetPropertyValue(value, ref guichengMulti, "GuichengMulti");
                    for (int i = 0; i < Categories.Count; i++)
                    {
                        Categories[i].GuichengMulti = GuichengMulti;
                        for (int j = 0; j < Categories[i].ErrorPoints.Count; j++)
                        {
                            Categories[i].ErrorPoints[j].GuichengMulti = GuichengMulti;
                        }
                    }
                }
            }
        }

    }
}
