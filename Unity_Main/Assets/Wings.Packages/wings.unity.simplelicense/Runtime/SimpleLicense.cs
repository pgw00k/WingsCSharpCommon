using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;

using GenOcean.Common;

namespace Wings
{
    /// <summary>
    /// 用以管理和控制授权的类
    /// </summary>
    public class SimpleLicense : SingletonManagerBase<SimpleLicenseManager>
    {
        public static DateTime Now
        {
            get
            {
                return Instance.GetNowTime();
            }
        }

        public static bool LicenseIsVaild
        {
            get
            {
                return Instance.CheckTimeLiencese();
            }
        }

        public static void UpdateTime()
        {
            Instance.UpdateTime();
        }

        public static void RegsiterSuccessAction(Action act)
        {
            Instance.RegsiterSuccessAction(act);
        }

        public static void RegsiterFailedAction(Action act)
        {
            Instance.RegsiterFailedAction(act);
        }

        public static void SetLicenseKey(byte[] key)
        {
            Instance.CurrentLicense = key;
        }
    }
}
