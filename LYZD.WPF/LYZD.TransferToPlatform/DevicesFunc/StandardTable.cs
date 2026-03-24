using LYZD.Utility.Log;
using LYZD.ViewModel;
using LYZD.ViewModel.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.TransferToPlatform.DevicesFunc
{
    public class StandardTable
    {
        public static void readStandardTable() {
            if (EquipmentData.DeviceManager.Devices.ContainsKey(DeviceName.标准表))
            {
                List<DeviceData> device = EquipmentData.DeviceManager.Devices[DeviceName.标准表]; //获得这个名称下的所有设备
                for (int i = 0; i < device.Count; i++)
                {
                    if (!device[i].Status)
                    {
                        return;
                    }
                }

                if (EquipmentData.ApplicationIsOver != true)
                {
                    try
                    {


                        //return;
                        float[] floatArray = EquipmentData.DeviceManager.Readstd();
                        if (floatArray.Count(a => a == 0) == floatArray.Length)  //全部都为0
                        {

                        }
                        if (floatArray != null && floatArray.Length > 28)
                        {
                            EquipmentData.StdInfo.Ua = floatArray[0];
                            EquipmentData.StdInfo.Ia = floatArray[1];


                            EquipmentData.StdInfo.Ub = floatArray[2];
                            EquipmentData.StdInfo.Ib = floatArray[3];

                            EquipmentData.StdInfo.Uc = floatArray[4];
                            EquipmentData.StdInfo.Ic = floatArray[5];

                            EquipmentData.StdInfo.PhaseUa = TrunPhase(floatArray[6]);
                            EquipmentData.StdInfo.PhaseIa = TrunPhase(floatArray[7]);


                            EquipmentData.StdInfo.PhaseUb = TrunPhase(floatArray[8]);
                            EquipmentData.StdInfo.PhaseIb = TrunPhase(floatArray[9]);

                            EquipmentData.StdInfo.PhaseUc = TrunPhase(floatArray[10]);
                            EquipmentData.StdInfo.PhaseIc = TrunPhase(floatArray[11]);

                            EquipmentData.StdInfo.Freq = floatArray[12];

                            EquipmentData.StdInfo.PF= floatArray[16];

                            //???上面有各个相位了，怎么这里还有一份？
                            EquipmentData.StdInfo.PhaseA = floatArray[12];
                            EquipmentData.StdInfo.PhaseB = floatArray[13];
                            EquipmentData.StdInfo.PhaseC = floatArray[14];

                            EquipmentData.StdInfo.Pa = floatArray[17];
                            EquipmentData.StdInfo.Qa = floatArray[18];


                            EquipmentData.StdInfo.Pb = floatArray[20];
                            EquipmentData.StdInfo.Qb = floatArray[21];


                            EquipmentData.StdInfo.Pc = floatArray[23];
                            EquipmentData.StdInfo.Qc = floatArray[24];

                            EquipmentData.StdInfo.Sa = floatArray[19];
                            EquipmentData.StdInfo.Sb = floatArray[22];
                            EquipmentData.StdInfo.Sc = floatArray[25];


                            EquipmentData.StdInfo.P = floatArray[26];
                            EquipmentData.StdInfo.Q = floatArray[27];
                            EquipmentData.StdInfo.S = floatArray[28];
                        }
                        else
                        {
                            if (floatArray != null)
                            {
                                LogManager.AddMessage("标准表数据解析失败,值：" + string.Join(",", floatArray), EnumLogSource.设备操作日志, EnumLevel.Warning); ;
                            }
                            else
                            {
                                LogManager.AddMessage("标准表数据解析失败,读回来值为空", EnumLogSource.设备操作日志, EnumLevel.Warning);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.AddMessage("标准表数据读取失败" + ex, EnumLogSource.设备操作日志, EnumLevel.Warning);
                    }
                }
            }
        }

        private static float TrunPhase(float p)
        {
            return 360.0f - p;
        }
    }
}
