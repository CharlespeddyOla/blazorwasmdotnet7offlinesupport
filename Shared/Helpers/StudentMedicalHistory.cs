using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppAcademics.Shared.Helpers
{
    public class StudentMedicalHistory
    {
        public int MEDHistoryID;
        public int SchInfoID;
        public int STDID;
        public int MEDID;
        public string MEDName;
        public bool MEDValue;
        public string MEDTextValue;

        public StudentMedicalHistory()
        {

        }

        public StudentMedicalHistory(int _MEDHistoryID, int _SchInfoID, int _STDID, int _MEDID, string _MEDName, bool _MEDValue, string _MEDTextValue)
        {
            MEDHistoryID = _MEDHistoryID;
            SchInfoID = _SchInfoID;
            STDID = _STDID;
            MEDID = _MEDID;
            MEDName = _MEDName;
            MEDValue = _MEDValue;
            MEDTextValue = _MEDTextValue;
        }
    }
}
