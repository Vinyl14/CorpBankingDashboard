namespace WebDashboard.Models;

public class MonthlySummaryDto
{
    public DateOnly AsOfDate { get; set; }
    public int TotalClients { get; set; }
    public int ActiveOacClients { get; set; }
    public int DormantClients { get; set; }
    public decimal TotalBalanceTab { get; set; }
    public decimal TotalBalanceGir { get; set; }
    public decimal TotalBalanceDep { get; set; }
    public decimal TotalDpkBalance { get; set; }
    public long TotalTrxFrequency { get; set; }
    public decimal TotalTrxVolume { get; set; }
    public decimal DigitalTrxVolume { get; set; }
    public decimal NondigitalTrxVolume { get; set; }
    public decimal TotalFbi { get; set; }
}

public class ProductInsightDto
{
    public string ProductCategory { get; set; } = string.Empty;
    public string ChannelType { get; set; } = string.Empty;
    public int ActiveCifCount { get; set; }
    public long TotalFrequency { get; set; }
    public decimal TotalVolume { get; set; }
    public decimal TotalOutstanding { get; set; }
}

public class RetentionSummaryDto
{
    public string RetentionStatus { get; set; } = string.Empty;
    public int ClientCount { get; set; }
    public decimal TotalVolume { get; set; }
    public decimal TotalBalance { get; set; }
}

public class EtlJobLogDto
{
    public long JobId { get; set; }
    public string BatchCode { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateOnly AsOfDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ExpectedRows { get; set; }
    public int ValidRowsLoaded { get; set; }
    public int RejectedRowsCount { get; set; }
    public long DurationMs { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime StartedAt { get; set; }
}

public class RejectedRecordDto
{
    public long RejectId { get; set; }
    public long JobId { get; set; }
    public int RowNumber { get; set; }
    public string? CifKey { get; set; }
    public string RejectReason { get; set; } = string.Empty;
    public string RawLineData { get; set; } = string.Empty;
}