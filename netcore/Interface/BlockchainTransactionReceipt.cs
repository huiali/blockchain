namespace Huiali.Blockchain.Interface
{
    public class BlockchainTransactionReceipt
    {
      
        public string TransactionHash { get; set; }
        public string TransactionIndex { get; set; }
        public string BlockHash { get; set; }
        public string BlockNumber { get; set; }
        public string CumulativeGasUsed { get; set; }
        public string GasUsed { get; set; }
        public string ContractAddress { get; set; }
        public string Status { get; set; }
        public string From { set; get; }
        public string To { set; get; }
    }
}
