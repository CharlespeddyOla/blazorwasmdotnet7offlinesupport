using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppAcademics.Shared.Models.Financials.Banks
{
    public class FINBankDetails
    {
        public int BankID { get; set; }
        public int SchInfoID { get; set; }
        public int AcctID { get; set; }
        public string BankAcctName { get; set; }
        public string AcctDescription { get; set; }
        public int BnkAcctTypeID { get; set; }
        public string BankAcctType { get; set; }
        public string BankAcctNumber { get; set; }
        public string Branch { get; set; }
        public int ChequeStartNo { get; set; }
        public int ChequeEndNo { get; set; }
        public int ChequeNo { get; set; }
        public int Id { get; set; }
    }
}
