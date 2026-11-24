window.dashboardCharts = {
    instances: {},
    renderBarChart: function (canvasId, labels, digitalData, nonDigitalData) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;
        if (this.instances[canvasId]) this.instances[canvasId].destroy();

        this.instances[canvasId] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    { label: 'Digital / General Volume (Rp)', data: digitalData, backgroundColor: '#38bdf8', borderRadius: 6 },
                    { label: 'Non-Digital Volume (Rp)', data: nonDigitalData, backgroundColor: '#f59e0b', borderRadius: 6 }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { labels: { color: '#cbd5e1' } } },
                scales: {
                    x: { ticks: { color: '#94a3b8' }, grid: { color: '#1e293b' } },
                    y: { ticks: { color: '#94a3b8' }, grid: { color: '#1e293b' } }
                }
            }
        });
    },
    renderDoughnutChart: function (canvasId, labels, values) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;
        if (this.instances[canvasId]) this.instances[canvasId].destroy();

        this.instances[canvasId] = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: values,
                    backgroundColor: ['#34d399', '#38bdf8', '#a78bfa'],
                    borderWidth: 0
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { position: 'bottom', labels: { color: '#cbd5e1' } } }
            }
        });
    }
};