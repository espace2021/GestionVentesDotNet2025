using Microsoft.ML.Data;

namespace GestionVentes.Models
{
    public class ProductOrderOutput
    {
        public string ProductName { get; set; }

        [ColumnName("Score")]
        public float PredictedOrderQty { get; set; }

    }
}
