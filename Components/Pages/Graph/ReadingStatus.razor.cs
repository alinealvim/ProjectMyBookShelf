using BlazorBootstrap;
using MyBookShelf.Services;

namespace MyBookShelf.Components.Pages.Graph
{
    public partial class ReadingStatus
    {
        private PieChart pieChart = default!;
        private PieChartOptions pieChartOptions = default!;
        private ChartData chartData = default!;

        private DateOnly startDate = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly endDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(3));

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PrepareGraph(true);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task PrepareGraph(bool initialize)
        {
            var groupedData = await GetData();

            var totalBooks = groupedData.Sum(g => g.Count);

            PieChartDataset dataset = new()
            {
                Label = "Status dos Livros",
                Data = groupedData.Select(g => (double?)g.Count / totalBooks * 100).ToList()
            };

            var labels = groupedData.Select(g => g.Status).ToList();

            chartData = new ChartData
            {
                Datasets = [dataset],
                Labels = labels
            };

            pieChartOptions = new PieChartOptions { Responsive = true };

            if (initialize)
            {
                await pieChart.InitializeAsync(chartData, pieChartOptions);
            }
            else
            {
                await pieChart.UpdateAsync(chartData, pieChartOptions);
            }
        }

        private async Task<List<ReadingStatusDTO>> GetData()
        {
            var userInfo = UserClaimService.GetUserInfo();

            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

            List<ReadingStatusDTO> groupedData = await ProgressService.GetReadingStatus(userInfo.UserId, startDateTime, endDateTime);

            return groupedData;
        }

        private async Task ApplyFilter()
        {
            await PrepareGraph(false);
        }
    }
}