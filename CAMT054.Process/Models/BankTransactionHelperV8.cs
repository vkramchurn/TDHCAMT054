//using CAMT054.Process.Models.V04;
using CAMT054.Process.Models.V08;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMT054.Process.Models
{
    public class BankTransactionHelperV8
    {
        private string _xmlContent { get; set; }
        private string _xsd4Path { get; set; }
        private string _xsd8Path { get; set; }
        private string _namespace { get; set; }
        public BankTransactionHelperV8(string content, string nspace)
        {
            _namespace = nspace;
            _xmlContent = content;
        }
        public IList<BankTransactionDto> GetBankTransaction<T>()
        {
            var result = new List<BankTransactionDto>();

            dynamic document = (T)Utils.Deserialize(_xmlContent, _namespace);
            var notifications = document.BkToCstmrDbtCdtNtfctn.Ntfctn;

            var entryNo = 0;
            var noTransaction = 0;

            foreach (var notif in notifications)
            {
                var iban = GetIban(notif.Acct?.Id);
                string dateRange = $"{Utils.GetDate(notif.FrToDt?.FrDtTm)} - {Utils.GetDate(notif.FrToDt?.ToDtTm)}";

                foreach (var entry in notif.Ntry)
                {
                    var operationDate = GetDateFromDateAndDateTimeChoice(entry.BookgDt);
                    var paquetId = GetPaquetID(entry.AddtlNtryInf);
                    var bankInfo = entry.AddtlNtryInf;

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
                            var bankDto = GetBankTransactionDto(txDetail, iban, operationDate, currency, bankAccount, dateRange, entryNo, noTransaction, paquetId, bankInfo);
                            result.Add(bankDto);
                        }
                    }
                }
            }

            return result;
        }
        private string GetDateFromDateAndDateTimeChoice(DateAndDateTime2Choice? dt)
        {
            var retval = "";
            if (dt?.ItemElementName == ItemChoiceType9.Dt)
            {
                retval = dt?.Item.Date.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return retval;
        }
        private decimal GetPaymentAmount(EntryTransaction10 tx)
        {
            var retval = tx.Amt.Value;
            if (tx.CdtDbtInd == CreditDebitCode.CRDT) retval *= -1;
            return retval;
        }
        private string GetKey(EntryTransaction10 tx)
        {
            var retval = "";
            StructuredRemittanceInformation16[] arrRemittance = tx.RmtInf?.Strd;
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
        private string GetBankInfo(string bankInfo)
        {
            return (bankInfo == null)? "" : bankInfo;
        }
        private BankTransactionDto GetBankTransactionDto(EntryTransaction10 txDetail, string iban, string operationDate, string currency, string bankAccount, string dateRange, int entryNo, int noTransaction, string paquetId, string bankInfo)
        {
            BankTransactionDto bankTransaction = new BankTransactionDto();
            bankTransaction.BankInfo = GetBankInfo(bankInfo);
            bankTransaction.EntryNo = entryNo;
            bankTransaction.NumberOfTransaction = noTransaction;

            PartyIdentification135 pIden = (txDetail.RltdPties?.Dbtr != null)? GetPartyIdentification135(txDetail.RltdPties?.Dbtr) : GetPartyIdentification135(txDetail.RltdPties?.UltmtDbtr);
            bankTransaction.IBAN = iban;
            if (operationDate != "") bankTransaction.OperationDate = DateTime.Parse(operationDate);
            bankTransaction.PaymentAmount = GetPaymentAmount(txDetail);
            bankTransaction.Currency = currency;
            bankTransaction.BankAccount = bankAccount;
            bankTransaction.DateRange = dateRange;

            var customerName = (pIden != null) ? pIden.Nm : "";
            var streetName = (pIden != null) ? pIden.PstlAdr?.StrtNm : "";
            var bldNumber = (pIden != null) ? pIden.PstlAdr?.BldgNb : "";
            var postCode = (pIden != null) ? pIden.PstlAdr?.PstCd : "";
            var townName = (pIden != null) ? pIden.PstlAdr?.TwnNm : "";
            var country = (pIden != null) ? pIden.PstlAdr?.Ctry : "";
            var addressLine = GetPostalAddressLine(pIden?.PstlAdr);

            bankTransaction.CustomerName = customerName;
            bankTransaction.DebitorIBAN = GetIban(txDetail.RltdPties?.DbtrAcct?.Id);//to workout
            bankTransaction.StreetName = streetName;
            var key = GetKey(txDetail);

            bankTransaction.BuildingNumber = bldNumber;//NOT PRESENT

            bankTransaction.PostCode = postCode;

            bankTransaction.TownName = townName;

            bankTransaction.Country = country;
            bankTransaction.AddressLine = addressLine;

            bankTransaction.KeyMapping = key;


            if (!string.IsNullOrEmpty(key) && key.Length >= 27)
            {
                bankTransaction.CodeAffectation = key.Substring(2, 5);
                bankTransaction.CodeCampagne = key.Substring(7, 5);
                bankTransaction.MouvementId = key.Substring(12, 14);
                bankTransaction.KeyFlag = key.Substring(26, 1);
            }
            bankTransaction.AddInfo = GetAdditionalInfo(txDetail.RmtInf?.Strd);

            //Customer Final Debitor
            if (customerName != null && customerName != "")
            {
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
        private PartyIdentification135 GetPartyIdentification135(Party40Choice p40choice)
        {
            PartyIdentification135 partyIdentification135 = null;
            if (p40choice != null)
            {
                if (p40choice.Item is PartyIdentification135 pIden && pIden!=null)
                {
                    partyIdentification135 = pIden;
                }
            }
            return partyIdentification135;
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
        private string GetAdditionalInfo(StructuredRemittanceInformation16[] strdField)
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
        private string GetPostalAddressLine(PostalAddress24 ptr)
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
            const string paquetIDLabel = "PAQUET ID:";

            int posIndex = sourceText.IndexOf(paquetIDLabel, StringComparison.Ordinal);

            if (posIndex == -1)
            {
                return "Not Found";
            }

            return sourceText.Substring(posIndex + paquetIDLabel.Length);
        }

    }
}
