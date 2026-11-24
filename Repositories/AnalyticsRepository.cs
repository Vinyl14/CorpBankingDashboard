using Npgsql;
using NpgsqlTypes;
using WebDashboard.Models;

namespace WebDashboard.Repositories;

public class AnalyticsRepository(NpgsqlDataSource dataSource)
{
    public async Task<List<DateOnly>> GetAvailablePeriodsAsync()
    {
        const string sql = "SELECT as_of_date FROM dwh.monthly_summary ORDER BY as_of_date DESC;";
        var list = new List<DateOnly>();
        await using var cmd = dataSource.CreateCommand(sql);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(reader.GetFieldValue<DateOnly>(0));
        }
        return list;
    }

    public async Task<MonthlySummaryDto?> GetMonthlySummaryAsync(DateOnly asOfDate)
    {
        const string sql = """
            SELECT as_of_date, total_clients, active_oac_clients, dormant_clients,
                   total_balance_tab, total_balance_gir, total_balance_dep, total_dpk_balance,
                   total_trx_frequency, total_trx_volume, digital_trx_volume, nondigital_trx_volume, total_fbi
            FROM dwh.monthly_summary
            WHERE as_of_date = @date;
            """;
        await using var cmd = dataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue("date", NpgsqlDbType.Date, asOfDate);
        await using var r = await cmd.ExecuteReaderAsync();
        if (!await r.ReadAsync()) return null;

        return new MonthlySummaryDto
        {
            AsOfDate = r.GetFieldValue<DateOnly>(0),
            TotalClients = r.GetInt32(1),
            ActiveOacClients = r.GetInt32(2),
            DormantClients = r.GetInt32(3),
            TotalBalanceTab = r.GetDecimal(4),
            TotalBalanceGir = r.GetDecimal(5),
            TotalBalanceDep = r.GetDecimal(6),
            TotalDpkBalance = r.GetDecimal(7),
            TotalTrxFrequency = r.GetInt64(8),
            TotalTrxVolume = r.GetDecimal(9),
            DigitalTrxVolume = r.GetDecimal(10),
            NondigitalTrxVolume = r.GetDecimal(11),
            TotalFbi = r.GetDecimal(12)
        };
    }

    public async Task<List<ProductInsightDto>> GetProductInsightsAsync(DateOnly asOfDate)
    {
        const string sql = """
            SELECT product_category, channel_type, active_cif_count, total_frequency, total_volume, total_outstanding
            FROM dwh.transactional_insight_monthly
            WHERE as_of_date = @date
            ORDER BY total_volume DESC;
            """;
        var list = new List<ProductInsightDto>();
        await using var cmd = dataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue("date", NpgsqlDbType.Date, asOfDate);
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new ProductInsightDto
            {
                ProductCategory = r.GetString(0),
                ChannelType = r.GetString(1),
                ActiveCifCount = r.GetInt32(2),
                TotalFrequency = r.GetInt64(3),
                TotalVolume = r.GetDecimal(4),
                TotalOutstanding = r.GetDecimal(5)
            });
        }
        return list;
    }

    public async Task<List<RetentionSummaryDto>> GetRetentionSummaryAsync(DateOnly asOfDate)
    {
        const string sql = """
            SELECT retention_status, COUNT(*)::INT, COALESCE(SUM(curr_month_vol), 0), COALESCE(SUM(curr_total_balance), 0)
            FROM dwh.repeat_transaction
            WHERE as_of_date = @date
            GROUP BY retention_status
            ORDER BY COUNT(*) DESC;
            """;
        var list = new List<RetentionSummaryDto>();
        await using var cmd = dataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue("date", NpgsqlDbType.Date, asOfDate);
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new RetentionSummaryDto
            {
                RetentionStatus = r.GetString(0),
                ClientCount = r.GetInt32(1),
                TotalVolume = r.GetDecimal(2),
                TotalBalance = r.GetDecimal(3)
            });
        }
        return list;
    }

    public async Task<List<EtlJobLogDto>> GetRecentJobLogsAsync(int limit = 15)
    {
        const string sql = """
            SELECT job_id, batch_code, file_name, as_of_date, status, expected_rows,
                   valid_rows_loaded, rejected_rows_count, duration_ms, error_message, started_at
            FROM audit.etl_job_logs
            ORDER BY job_id DESC
            LIMIT @limit;
            """;
        var list = new List<EtlJobLogDto>();
        await using var cmd = dataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue("limit", limit);
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new EtlJobLogDto
            {
                JobId = r.GetInt64(0),
                BatchCode = r.GetString(1),
                FileName = r.GetString(2),
                AsOfDate = r.GetFieldValue<DateOnly>(3),
                Status = r.GetString(4),
                ExpectedRows = r.GetInt32(5),
                ValidRowsLoaded = r.GetInt32(6),
                RejectedRowsCount = r.GetInt32(7),
                DurationMs = r.GetInt64(8),
                ErrorMessage = r.IsDBNull(9) ? null : r.GetString(9),
                StartedAt = r.GetDateTime(10)
            });
        }
        return list;
    }

    public async Task<List<RejectedRecordDto>> GetRejectedRecordsByJobAsync(long jobId)
    {
        const string sql = """
            SELECT reject_id, job_id, row_number, cif_key, reject_reason, raw_line_data
            FROM audit.etl_rejected_records
            WHERE job_id = @jobId
            ORDER BY row_number ASC;
            """;
        var list = new List<RejectedRecordDto>();
        await using var cmd = dataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue("jobId", jobId);
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new RejectedRecordDto
            {
                RejectId = r.GetInt64(0),
                JobId = r.GetInt64(1),
                RowNumber = r.GetInt32(2),
                CifKey = r.IsDBNull(3) ? "-" : r.GetString(3),
                RejectReason = r.GetString(4),
                RawLineData = r.GetString(5)
            });
        }
        return list;
    }
}