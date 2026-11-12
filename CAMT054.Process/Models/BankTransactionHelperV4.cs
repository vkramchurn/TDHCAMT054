using CAMT054.Process.Models.V04;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CAMT054.Process.Models
{
    public class BankTransactionHelperV4
    {
        private string _xmlContent { get; set; }
        private string _xsd4Path { get; set; }
        private string _xsd8Path { get; set; }
        private string _namespace { get; set; }
        public BankTransactionHelperV4(string content,string nspace) { 
            _namespace = nspace;
            _xmlContent = content;
        }
        public IList<BankTransactionDto> GetBankTransaction<T>()
        {
            var result = new List<BankTransactionDto>();

            dynamic document = (T)Utils.Deserialize(_xmlContent, _namespace);
            var notifications = document.BkToCstmrDbtCdtNtfctn.Ntfctn;

            foreach (var notif in notifications)
            {
                var iban = GetIban(notif.Acct?.Id);
                string dateRange = $"{Utils.GetDate(notif.FrToDt?.FrDtTm)} - {Utils.GetDate(notif.FrToDt?.ToDtTm)}";

                var entryNo = 0;
                var noTransaction = 0;
                

                foreach (var entry in notif.Ntry)
                {
                    var paquetId = GetPaquetID(entry.AddtlNtryInf);
                    var bankInfo = entry.AddtlNtryInf;
                    var operationDate = GetDateFromDateAndDateTimeChoice(entry.BookgDt);

                    var amount = entry.Amt?.Value ?? 0m;
                    var currency = entry.Amt?.Ccy;
                    var bankAccount = entry.NtryRef;

                    if (entry.NtryDtls == null)
                        continue;

                    foreach (var ntryDtls in entry.NtryDtls)
                    {
                        noTransaction = Utils.IfNotNullConvertToInt(ntryDtls.Btch?.NbOfTxs);
                        if (ntryDtls.TxDtls == null) continue;
                        foreach (var txDetail in ntryDtls.TxDtls)
                        {
                            entryNo++;
                            var bankDto = GetBankTransactionDto(txDetail, iban, operationDate, currency, bankAccount, dateRange,entryNo, noTransaction, paquetId,bankInfo);
                            result.Add(bankDto);
                        }
                    }
                }
            }

            return result;
        }
        private string GetDateFromDateAndDateTimeChoice(DateAndDateTimeChoice? dt)
        {
            var retval = "";
            if (dt?.ItemElementName == ItemChoiceType8.Dt)
            {
                retval = dt?.Item.Date.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return retval;
        }
        private decimal GetPaymentAmount(EntryTransaction4 tx)
        {
            var retval = tx.Amt.Value;
            if (tx.CdtDbtInd == CreditDebitCode.CRDT) retval *= -1;
            return retval;
        }
        private string GetKey(EntryTransaction4 tx)
        {
            var retval = "";
            StructuredRemittanceInformation9[] arrRemittance = tx.RmtInf?.Strd;
            if (arrRemittance != null && arrRemittance.Length > 0)
            {
                foreach (var remittance in arrRemittance)
                {
                    retval = remittance.CdtrRefInf?.Ref;
                }
            }
            if (retval == "")
            {
                if (tx.RmtInf?.Ustrd != null && tx.RmtInf?.Ustrd.Length > 0)
                {
                    foreach (var val in tx.RmtInf?.Ustrd)
                    {
                        retval = val.ToString();
                    }
                }
            }
            return retval;
        }
        private BankTransactionDto GetBankTransactionDto(EntryTransaction4 txDetail, string iban, string operationDate, string currency, string bankAccount, string dateRange, int entryNo, int noTransaction, string paquetId, string bankInfo)
        {
            BankTransactionDto bankTransaction = new BankTransactionDto();
            bankTransaction.BankInfo = (bankInfo != null) ? bankInfo : "";
            bankTransaction.EntryNo = entryNo;
            bankTransaction.NumberOfTransaction = noTransaction;
            bankTransaction.IBAN = iban;
            if (operationDate != "") bankTransaction.OperationDate = DateTime.Parse(operationDate);
            bankTransaction.PaymentAmount = GetPaymentAmount(txDetail);
            bankTransaction.Currency = currency;
            bankTransaction.BankAccount = bankAccount;
            bankTransaction.DateRange = dateRange;

            var customerName = txDetail.RltdPties?.Dbtr?.Nm ?? txDetail.RltdPties?.UltmtDbtr?.Nm;

            PostalAddress6 postalAddress6 = txDetail.RltdPties?.Dbtr?.PstlAdr ?? txDetail.RltdPties?.UltmtDbtr?.PstlAdr;

            /*
            var streetName = txDetail.RltdPties?.Dbtr?.PstlAdr?.StrtNm ?? txDetail.RltdPties?.UltmtDbtr?.PstlAdr?.StrtNm;
            var bldNumber = txDetail.RltdPties?.Dbtr?.PstlAdr?.BldgNb ?? txDetail.RltdPties?.UltmtDbtr?.PstlAdr?.BldgNb;
            var postCode = txDetail.RltdPties?.Dbtr?.PstlAdr?.PstCd ?? txDetail.RltdPties?.UltmtDbtr?.PstlAdr?.PstCd;
            var townName = txDetail.RltdPties?.Dbtr?.PstlAdr?.TwnNm ?? txDetail.RltdPties?.UltmtDbtr?.PstlAdr?.TwnNm;
            var country = txDetail.RltdPties?.Dbtr?.PstlAdr?.Ctry ?? txDetail.RltdPties?.UltmtCdtr?.PstlAdr?.Ctry;
            var addressLine = GetPostalAddressLine(txDetail.RltdPties?.Dbtr?.PstlAdr);
            */
            var streetName = postalAddress6?.StrtNm;
            var bldNumber = postalAddress6?.BldgNb;
            var postCode = postalAddress6?.PstCd;
            var townName = postalAddress6?.TwnNm;
            var country = postalAddress6?.Ctry;
            var addressLine = GetPostalAddressLine(postalAddress6);

            bankTransaction.CustomerName = customerName;
            bankTransaction.DebitorIBAN = GetIban(txDetail.RltdPties?.DbtrAcct?.Id);//to workout
            bankTransaction.StreetName = streetName;
            var key = GetKey(txDetail);

            bankTransaction.BuildingNumber = bldNumber;
            bankTransaction.PostCode = postCode;
            bankTransaction.TownName = townName;
            bankTransaction.Country = country;
            bankTransaction.AddressLine = addressLine;

            bankTransaction.KeyMapping = key;

            //bankTransaction.BankInfo = txDetail.RmtInf?.Ustrd?.FirstOrDefault();

            if (!string.IsNullOrEmpty(key) && key.Length >= 27)
            {
                bankTransaction.CodeAffectation = key.Substring(2, 5);
                bankTransaction.CodeCampagne = key.Substring(7, 5);
                bankTransaction.MouvementId = key.Substring(12, 14);
                bankTransaction.KeyFlag = key.Substring(26, 1);
            }
            bankTransaction.AddInfo = GetAdditionalInfo(txDetail.RmtInf?.Strd);

            //Customer Final Debitor
            if(customerName!= null && customerName !="") {
                bankTransaction.FinalDebitorName = customerName;
                bankTransaction.FinalDebitorStreet = streetName;
                bankTransaction.FinalDebitorBuilding = bldNumber;
                bankTransaction.FinalDebitorPostCode = postCode;
                bankTransaction.FinalDebitorTown = townName;
                bankTransaction.FinalDebitorCountry = country;
                bankTransaction.FinalDebitorAddressLine = addressLine;
            }
            bankTransaction.TransactionCurrency = currency;
            bankTransaction.PaquetId = paquetId;

            return bankTransaction;
        }
        private string GetIban(AccountIdentification4Choice id)
        {
            var retval = "";
            if (id != null)
            {
                if (id.Item is string iban && !string.IsNullOrWhiteSpace(iban))
                {
                    retval = iban;
                }
            }
            return retval;
        }
        private string GetAdditionalInfo(StructuredRemittanceInformation9[] strdField)
        {
            var add = "";
            var retval = "";
            if (strdField != null && strdField.Length > 0)
            {
                foreach (var str in strdField)
                {
                    if (str.AddtlRmtInf != null && str.AddtlRmtInf.Length > 0)
                    {
                        foreach (var addtl in str.AddtlRmtInf)
                        {
                            if (!string.IsNullOrEmpty(addtl) && addtl != "?REJECT?0" && addtl != "?ERROR?000")
                            {
                                if (add != "") add += ";";
                                add += addtl;
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(add) &&
                add != "?REJECT?0" &&
                add != "?ERROR?000")
            {
                retval = add;
            }
            return retval;
        }
        private string GetPostalAddressLine(PostalAddress6 ptr)
        {
            string adrLine = "";
            if (ptr != null)
            {
                if (ptr.AdrLine != null && ptr.AdrLine.Length > 0)
                {
                    foreach (var adr in ptr.AdrLine)
                    {
                        if (adrLine != "") adrLine += ", ";
                        adrLine += adr;
                    }
                }
            }
            return adrLine;
        }
        private string GetPaquetID(string sourceText)
        {
            string paquetId = "Not Found";
            if (sourceText != null)
            {
                const string paquetIDLabel = "PAQUET ID:";

                int posIndex = sourceText.IndexOf(paquetIDLabel, StringComparison.Ordinal);

                if (posIndex == -1)
                {
                    return "Not Found";
                }
                paquetId = sourceText.Substring(posIndex + paquetIDLabel.Length);
            }

            return paquetId;
        }

    }
}
