using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

using GenOcean.Common;

using Wings.Extension;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Wings
{
    public class SimpleLicenseManager : ManagerBase
    {
        public string TimeStampUrl = @"https://time.weather.gov.hk/cgi-bin/time5a.pr?a=1";
        public byte[] CurrentLicense;

        protected HttpClient _TimeHttpClient;
        protected DateTime _Time = DateTime.Now;
        protected DateTime _LocalTimeStart = new DateTime(1970, 1, 1);

        protected Action _OnUpdateTimeSuccess = null;
        protected Action _OnUpdateTimeFailed = null;

        protected byte[] _CheckBytes;

        protected byte _XorKey = 0x57;

        public override void Init()
        {
            base.Init();
            _TimeHttpClient = new HttpClient();
            //_TimeHttpClient.Timeout = TimeSpan.FromMilliseconds(3000);
        }

        public virtual DateTime GetNowTime()
        {
            return _Time;
        }

        public virtual void UpdateTime()
        {
#if UNITY_EDITOR
            Debug.Log($"Update Time...");
#endif
            _TimeHttpClient.GetAsync(TimeStampUrl).ContinueWith(GetNowTimeResult);
        }

        public virtual void RegsiterSuccessAction(Action act)
        {
            _OnUpdateTimeSuccess += act;
        }

        public virtual void RegsiterFailedAction(Action act)
        {
            _OnUpdateTimeFailed += act;
        }

        public virtual void GetNowTimeResult(Task<HttpResponseMessage> res)
        {

            bool IsUpdateSuccess = false;

            /*
             * 1Tick 为 100 ns，也就是 1Tick=10000 ms
             */
            if(res.IsCompleted)
            {
                try
                {
                    string r = res.Result.Content.ReadAsStringAsync().Result;
                    long ltime = long.Parse(r.Substring(2));
                    TimeSpan ts = new TimeSpan(ltime * 10000);
                    _Time = TimeZoneInfo.ConvertTimeFromUtc(_LocalTimeStart.Add(ts), TimeZoneInfo.Local);
                    IsUpdateSuccess = true;
#if UNITY_EDITOR
                    Debug.Log($" Now Time : {_Time:yyyy_MM_dd-HH_mm_ssss}");
#endif
                }catch(Exception err)
                {
                    IsUpdateSuccess = false;
#if UNITY_EDITOR
                    Debug.LogError($"Update Time failed : {err.Message}");
#endif
                }

                finally
                {
                    res.Dispose();
                }

            }

            if(IsUpdateSuccess)
            {
                try
                {
                    _OnUpdateTimeSuccess.Invoke();
                }catch(Exception err)
                {
#if UNITY_EDITOR
                    Debug.LogError($"Invoke  _OnUpdateTimeSuccess : {err.Message}");
#endif
                }
            }
            else
            {
                try
                {
                    _OnUpdateTimeFailed.Invoke();
                }
                catch (Exception err)
                {
#if UNITY_EDITOR
                    Debug.LogError($"Invoke  _OnUpdateTimeFailed : {err.Message}");
#endif
                }
            }
        }

        public virtual bool CheckTimeLiencese()
        {
            bool isOk = false;
            
            try
            {
                _CheckBytes = new byte[CurrentLicense.Length];
                for(int i =0;i<CurrentLicense.Length;i++)
                {
                    _CheckBytes[i] = (byte)(CurrentLicense[i] ^ _XorKey);
                }

                long ltime = BitConverter.ToInt64(_CheckBytes);
                TimeSpan ts = new TimeSpan(ltime * 10000);
                DateTime dt = TimeZoneInfo.ConvertTimeFromUtc(_LocalTimeStart.Add(ts), TimeZoneInfo.Local);
                TimeSpan ts2 = _Time.Subtract(dt);

                isOk = ts2.TotalDays < 5 && ts2.TotalDays > 0;

#if UNITY_EDITOR
                Debug.Log($"CheckTimeLiencese : {ts2.TotalDays}");
#endif

            }
            catch (Exception err)
            {
                isOk = false;
            }

            return isOk;
        }

    }
}
