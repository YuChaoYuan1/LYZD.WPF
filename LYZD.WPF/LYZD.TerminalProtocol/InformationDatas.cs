using System.Collections.Generic;
namespace LYZD.TerminalProtocol
{
    public class InformationDatas
    {
        private Dictionary<int, InformationData> _ValueTable = new Dictionary<int, InformationData>();
        public Dictionary<int, InformationData> ValueTable
        {
            get { return _ValueTable; }
            set { _ValueTable = value; }
        }
        /// <summary>
        /// 取点表数据
        /// </summary>
        /// <param name="dataKey">终端实际点号</param>
        /// <returns></returns>
        public InformationData Get(int dataKey)
        {
            if (ValueTable.ContainsKey(dataKey))
            {
                return ValueTable[dataKey];
            }
            return new InformationData(true);
        }
        public void Set(int dataKey, InformationData data)
        {
            if (ValueTable.ContainsKey(dataKey))
            {
                ValueTable[dataKey] = data;
            }
            else
            {
                ValueTable.Add(dataKey, data);
            }
        }

    }
}
