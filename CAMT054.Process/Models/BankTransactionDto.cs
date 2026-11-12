using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMT054.Process.Models
{

    public class BankTransactionDto
    {
        public string IBAN { get; set; }
        public DateTime OperationDate { get; set; }
        public string KeyMapping { get; set; }
        public decimal PaymentAmount { get; set; }
        public string Currency { get; set; }

        public string CustomerName { get; set; }
        public string DebitorIBAN { get; set; }

        public string StreetName { get; set; }
        public string BuildingNumber { get; set; }
        public string PostCode { get; set; }
        public string TownName { get; set; }
        public string Country { get; set; }
        public string AddressLine { get; set; }

        public string FinalDebitorName { get; set; }
        public string FinalDebitorStreet { get; set; }
        public string FinalDebitorBuilding { get; set; }
        public string FinalDebitorPostCode { get; set; }
        public string FinalDebitorTown { get; set; }
        public string FinalDebitorCountry { get; set; }
        public string FinalDebitorAddressLine { get; set; }

        public string CodeAffectation { get; set; }
        public string CodeCampagne { get; set; }
        public string MouvementId { get; set; }
        public string KeyFlag { get; set; }

        public string AddInfo { get; set; }

        public string BankAccount { get; set; }
        public string DateRange { get; set; }

        public string SourceCurrency { get; set; }
        public decimal? SourceAmount { get; set; }
        public decimal? ExchangeRate { get; set; }

        public string BankInfo { get; set; }

        public int EntryNo { get; set; }
        public string TransactionCurrency { get; set; }
        public string PaquetId { get; set; }
        public int NumberOfTransaction { get; set; }
        public int ImportNumber { get; set; }
    }

}
