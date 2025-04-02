
using Microsoft.ML.Data;

namespace GestionVentes.Models
{
    public class ProductOrderPrediction
    {
        [LoadColumn(0)]
        public int ProductID { get; set; }

        [LoadColumn(1)]
        public float OrderQty { get; set; }

        [LoadColumn(2)]
        public float ListPrice { get; set; }

        [LoadColumn(3)]
        public string Category { get; set; }

        [LoadColumn(4)]
        public float LineItemTotal { get; set; }
    }

}
