/* Recent Orders */
var barChartData = {
  labels : ["Jun 02.2021","Jul 05.2021","Aug 15.2021","Sep 03.2021","Sep 23.2021","Oct 03.2021"],
  datasets : [
      {
        barThickness: 30,
        hidden: false,
        borderWidth: 2,
        label: 'HGB (g/l)',
        backgroundColor: "#B58AF2",
        borderColor: "#8149FB",
        pointBackgroundColor: '#fff',
        pointBorderColor: "#8149FB",
        pointHighlightStroke: "#fff",
        data : [200,300,250,460,150,300]
      },
      {
        barThickness: 50,
        borderWidth: 2.2,
        hidden: true,
        label: 'WBC (k/µl)',
        backgroundColor: "#1BA1FB",
        borderColor: "#49A8FB",
        pointBackgroundColor: '#fff',
        pointBorderColor: "#1BA1FB",
        pointHighlightStroke: "#fff",
        data : [40000,55000,60000,40000,95340,82350]
      },
      {
        barThickness: 50,
        hidden: true,
        borderWidth: 2.2,
        label: 'GRAN (k/µl)',
        backgroundColor: "D959E0",
        borderColor: "#D959E0",
        pointBackgroundColor: '#fff',
        pointBorderColor: "#D959E0",
        pointHighlightStroke: "#fff",
        data : [250,520,320,310,480,420,350]
      },
      {
        barThickness: 50,
        hidden: true,
        borderWidth: 2.2,
        label: 'Cr (µmol/l)',
        backgroundColor: "#5BD4EC",
        borderColor: "#FFC644",
        pointBackgroundColor: '#fff',
        pointBorderColor: "#FFC644",
        pointHighlightStroke: "#fff",
        suggestedMax: 10000,
        data : [250,120,220,310,380,320]
      },
      {
        barThickness: 50,
        hidden: true,
        borderWidth: 2.2,
        label: 'CrCl (ml/min)',
        backgroundColor: "#5EE42F",
        borderColor: "#5EE42F",
        pointBackgroundColor: '#fff',
        pointBorderColor: "#5EE42F",
        pointHighlightStroke: "#fff",
        data : [100,250,280,130,240,150,90]
      }
  ]
}

var ctx1 = document.getElementById("myChart").getContext("2d");
window.myBar = new Chart(ctx1, {
type: 'line',
data: barChartData,
options: {
  animation: false,
  maintainAspectRatio: false,
  responsive: true,
  plugins: {
    legend: {
      display: true,
      onClick: 
        function test(e, legendItem, legend) {
          const index = legendItem.datasetIndex;
          const ci = legend.chart;
          const allItems = legend.legendItems;

          allItems.forEach(el => {
            if (ci.isDatasetVisible(index)) {
              ci.show(index);
              legendItem.hidden = false;
            } else {
              ci.hide(el.datasetIndex);
              legendItem.hidden = true;
            }
          });                    
          ci.show(index);
          legendItem.hidden = true
        },
        onHover: (e) => {
          e.native.target.style.cursor = 'pointer';
      },
      labels: {
        boxHeight: 1.2,
        boxWidth: 30,
        color: '#444',
        font: {
          size: 14
        }
      },
    },
    title: {
      align: 'center',
      display: true,
      text: "Laboratory Chart",
      color: '#000',
      font: {
        size: '18px'
      }
    },
    tooltip: {
      title: false,
      displayColors: false,
      bodyFont: {
        size: 16
      }
    }
  },
  scales: {
    y: {
        grid: {          
          borderColor: '#D5D7DC',
          borderColor: 'rgba(0,0,0,0)',
          borderDash: [3,3],
          tickColor: 'rgba(0,0,0,0)',
          color: "#cecece"
        },
        ticks: {
          color: '#444',

        },
        suggestedMin: 0,
    },
    x: {
      grid: {
        borderColor: 'rgba(0,0,0,0)',
        borderDash: [3,3],
        tickColor: 'rgba(0,0,0,0)',
        color: "#cecece"
      },
      ticks: {
        color: '#444',
        borderWidth: 0
      }
    }
  }	
}
});

