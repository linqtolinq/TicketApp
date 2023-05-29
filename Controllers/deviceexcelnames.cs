namespace TicketAppApi.Controllers
{
    internal class deviceexcelnames
    {
        public string serialNumber { get; set; } = "流水编号";
        public string documentNumber { get; set; } = "单据编号";
        public string flowType { get; set; } = "流水类别";
        public string inAmount { get; set; } = "入账金额";
        public string outAmount { get; set; } = "出账金额";
        public string financeManager { get; set; } = "管理员名称";
        public string createTime { get; set; } = "创建时间";
        public string remark { get; set; } = "备注";
    }
}