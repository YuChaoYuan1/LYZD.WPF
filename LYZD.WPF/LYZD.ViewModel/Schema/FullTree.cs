using LYZD.DAL;
using LYZD.DAL.DataBaseView;
using LYZD.ViewModel.CodeTree;
using LYZD.ViewModel.Schema;
using System.Collections.Generic;
using System.Linq;

namespace LYZD.ViewModel.Schema
{
    public class FullTree : SchemaNodeViewModel
    {
        private static FullTree instance = null;
        public static FullTree Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FullTree();
                }
                return instance;
            }
        }
        public static Dictionary<string, string> ApplicabilityList;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            //在这里加上适用性--376.1--698.45--104 --保存数据呢还是啥呢
            if (ApplicabilityList==null)
            {
                ApplicabilityList = new Dictionary<string, string>();
                List<DynamicModel> models = DALManager.ApplicationDbDal.GetList(EnumAppDbTable.T_SCHEMA_PARA_FORMAT.ToString());
                string ParaNo = "";
                string value = "";
                for (int i = 0; i < models.Count; i++)
                {
                    ParaNo = models[i].GetProperty("PARA_NO") as string;
                    value = models[i].GetProperty("PARA_APPLY") as string;
                    if (!ApplicabilityList.ContainsKey(ParaNo))
                    {
                        ApplicabilityList.Add(ParaNo, value);
                    }
                }
                
           
            }

            //List<SchemaNodeViewModel> tem = new List<SchemaNodeViewModel>();
            Dictionary<string, SchemaNodeViewModel> tem = new Dictionary<string, SchemaNodeViewModel>();
            CodeTreeNode nodeSchemaRoot = CodeTreeViewModel.Instance.CodeNodes.FirstOrDefault(item => item.CODE_TYPE == "SchemaCategory");
            for (int i = 0; i < nodeSchemaRoot.Children.Count; i++)
            {
                SchemaNodeViewModel nodeTemp = GetNode(nodeSchemaRoot.Children[i]);
                if (nodeTemp != null)
                {
                    tem.Add(nodeTemp.Name,nodeTemp);
                    SchemaFramework.AddNewPair(nodeTemp.ParaNo, nodeTemp.Name);
                    SetParaNo(nodeTemp);
                }
            }
           
            List<string> list = new List<string>();
            for (int i = 0; i < 999; i++)
            {
                string str = ViewModel.Const.OperateFile.GetINI("Sort", i.ToString(), System.IO.Directory.GetCurrentDirectory() + "\\Ini\\ShcemaShor.ini").Trim();
                if (str!="")
                {
                    list.Add(str);
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (tem.ContainsKey(list[i]))
                {
                    Children.Add(tem[list[i]]);
                    tem.Remove(list[i]);
                }
            }

            foreach (var item in tem.Keys)
            {
                Children.Add(tem[item]);
            }
   

            //CodeTreeNode nodeSchemaRoot = CodeTreeViewModel.Instance.CodeNodes.FirstOrDefault(item => item.CODE_TYPE == "SchemaCategory");
            //for (int i = 0; i < nodeSchemaRoot.Children.Count; i++)
            //{
            //    SchemaNodeViewModel nodeTemp = GetNode(nodeSchemaRoot.Children[i]);
            //    if (nodeTemp != null)
            //    {
            //        Children.Add(nodeTemp);
            //        SchemaFramework.AddNewPair(nodeTemp.ParaNo, nodeTemp.Name);
            //        SetParaNo(nodeTemp);
            //    }
            //}
        }


        /// <summary>
        /// 获取方案节点
        /// </summary>
        /// <param name="codeNode"></param>
        /// <returns></returns>
        private SchemaNodeViewModel GetNode(CodeTreeNode codeNode)
        {
            if (!codeNode.CODE_ENABLED)
            {
                return null;
            }
            SchemaNodeViewModel schemaNode = new SchemaNodeViewModel();
            schemaNode.Name = codeNode.CODE_NAME;
            #region 设置当前节点的信息

            if (codeNode.CODE_LEVEL == 2)
            {
                schemaNode.ParaNo = codeNode.CODE_VALUE.PadLeft(2, '0');
            }
            else
            {
                schemaNode.ParaNo = codeNode.CODE_VALUE.PadLeft(3, '0');
            }
  

            //根据编号拿取没有问题

            schemaNode.Level = codeNode.CODE_LEVEL - 1;
            #endregion
            for (int i = 0; i < codeNode.Children.Count; i++)
            {
                SchemaNodeViewModel schemaNodeChild = GetNode(codeNode.Children[i]);
                if (schemaNodeChild != null)
                {
                    schemaNode.Children.Add(schemaNodeChild);
                    schemaNodeChild.ParentNode = schemaNode;
                }
            }
            if (schemaNode.Children.Count == 0)
            {
                schemaNode.IsTerminal = true;
            }
            else
            {
                schemaNode.IsTerminal = false;
            }
            return schemaNode;
        }
        /// <summary>
        /// 设置节点编号
        /// </summary>
        /// <param name="nodeTemp"></param>
        private void SetParaNo(SchemaNodeViewModel nodeTemp)
        {
            if (nodeTemp.ParentNode != null)
            {
                nodeTemp.ParaNo = nodeTemp.ParentNode.ParaNo + nodeTemp.ParaNo;
                if (ApplicabilityList.ContainsKey(nodeTemp.ParaNo))//添加他的适用性
                {
                    nodeTemp.Applicability = ApplicabilityList[nodeTemp.ParaNo];
                }
                SchemaFramework.AddNewPair(nodeTemp.ParaNo, nodeTemp.Name);
                if (nodeTemp.IsTerminal)
                {
                    nodeTemp.ViewNo = ResultViewHelper.GetParaNoView(nodeTemp.ParaNo);
                    DynamicModel modelTemp = SchemaFramework.GetParaFormat(nodeTemp.ParaNo);
                    if (modelTemp != null)
                    {
                        nodeTemp.SortNo = modelTemp.GetProperty("DEFAULT_SORT_NO") as string;     //排序的编号
                    }
                    if (string.IsNullOrEmpty(nodeTemp.SortNo))
                    {
                        nodeTemp.SortNo = "999";
                    }
                }
            }

            for (int i = 0; i < nodeTemp.Children.Count; i++)
            {
                SetParaNo(nodeTemp.Children[i]);
            }
        }
    }
}
