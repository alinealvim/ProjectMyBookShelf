using BlazorBootstrap;
using MyBookShelf.Services;

namespace MyBookShelf.Components.Pages.Graph
{
    public partial class PageStatus
    {
        private BarChart barChart = default!;
        private BarChartOptions barChartOptions = default!;
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

            BarChartDataset dataset = new()
            {
                Label = "Páginas Lidas",
                Data = groupedData.Select(g => (double?)g.PagesRead).ToList()
            };

            var labels = groupedData.Select(g => g.Date).ToList();

            chartData = new ChartData
            {
                Datasets = [dataset],
                Labels = labels
            };
            barChartOptions = new BarChartOptions { Responsive = true, Interaction = new Interaction { Mode = InteractionMode.Index } };

            if (initialize)
            {
                await barChart.InitializeAsync(chartData, barChartOptions);
            }
            else
            {
                await barChart.UpdateAsync(chartData, barChartOptions);
            }
        }

        private async Task<List<PageStatusDTO>> GetData()
        {
            var userInfo = UserClaimService.GetUserInfo();

            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

            List<PageStatusDTO> groupedData = await ProgressService.GetPageStatus(userInfo.UserId, startDateTime, endDateTime);

            return groupedData;
        }

        private async Task ApplyFilter()
        {            
            await PrepareGraph(false);            
        }
    }
}