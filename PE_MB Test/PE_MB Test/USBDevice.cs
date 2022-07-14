using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PE_MB_Test
{
    public class USBDevice
    {
        bool isInserted;
        string Vid_Pid;
        public string expectedVid_Pid;
        public string expectedName;
        public USBDevice()
        {
            isInserted = false;
            expectedName = "";
        }

        public bool InsertCheckId(string givenVid_Pid)
        {
            bool returnValue = false;
            string OnlyVid = "";
            if (givenVid_Pid.Length > 0 && givenVid_Pid.Contains("VID_"))
            {
                OnlyVid = givenVid_Pid.Substring(givenVid_Pid.IndexOf("VID_"), 8);
                if (OnlyVid == expectedVid_Pid)
                {
                    Vid_Pid = givenVid_Pid;
                    isInserted = true;
                    returnValue = true;
                }
            }
            return returnValue;
        }

        public bool InsertCheckIdAndName(string givenVid_Pid, string GivenName)
        {
            bool returnValue = false;
            string OnlyVid = "";
            if (givenVid_Pid.Length > 0 && givenVid_Pid.Contains("VID_") && GivenName.Length > 0)
            {
                OnlyVid = givenVid_Pid.Substring(givenVid_Pid.IndexOf("VID_"), 8);
                if (OnlyVid == expectedVid_Pid && GivenName == expectedName)
                {
                    Vid_Pid = givenVid_Pid;
                    isInserted = true;
                    returnValue = true;
                }
            }
            return returnValue;
        }

        public bool InsertCheck(string givenVid_Pid, string GivenName)
        {
            bool returnValue = false;
            if(givenVid_Pid.Length > 0 && GivenName.Length > 0)
            if (givenVid_Pid.Contains(expectedVid_Pid) && GivenName == expectedName)
                {
                    Vid_Pid = givenVid_Pid;
                    isInserted = true;
                    returnValue = true;
                }
            return returnValue;
        }

        public bool RemoveCheckId(string givenVid_Pid)
        {
            bool returnValue = false;

            if (givenVid_Pid == Vid_Pid)
            {
                Vid_Pid = "";
                isInserted = false;
                returnValue = true;
            }
            return returnValue;
        }

        public bool checkIfInserted()
        {
            bool returnValue = false;
            if(isInserted)
            {
                returnValue = true;
            }
            return returnValue;
        }
        public void GetExpextedVid(string Vid)
        {
            expectedVid_Pid = Vid;
        }
        public void GetExpextedName(string Name_tmp)
        {
            expectedName = Name_tmp;
        }
    }
}
