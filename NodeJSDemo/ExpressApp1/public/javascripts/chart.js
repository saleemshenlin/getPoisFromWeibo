$(function() {
    var drawWeiboCountByMonth = function(data) {
        $('.weibo-chart').highcharts({
            chart: {
                zoomType: 'xy'
            },
            credits: {
                enabled: false
            },
            exporting: {
                enabled: true,
                sourceWidth: 1200
            },
            title: {
                text: ''
            },
            xAxis: [{
                categories: ['周一', '周二', '周三', '周四', '周五', '周六', '周日'],
                tickInterval: 1,
                labels: {
                    style: {
                        'font-size': '18px'
                    }
                }
            }],
            yAxis: [{ // Primary yAxis
                labels: {
                    format: '{value}',
                    style: {
                        'font-size': '18px',
                        color: '#9c27b0'
                    }
                },
                title: {
                    text: '签到照片数',
                    style: {
                        'font-size': '18px',
                        color: '#9c27b0'
                    }
                }
            }, { // Secondary yAxis
                title: {
                    text: '签到微博数',
                    style: {
                        'font-size': '18px',
                        color: '#2196f3'
                    }
                },
                labels: {
                    format: '{value}',
                    style: {
                        'font-size': '18px',
                        color: '#2196f3'
                    }
                },
                opposite: true
            }],
            tooltip: {
                shared: true
            },
            legend: {
                layout: 'vertical',
                align: 'left',
                x: 120,
                verticalAlign: 'top',
                y: 50,
                floating: true,
                backgroundColor: '#FFFFFF',
                itemStyle: {
                    'font-size': '18px'
                }
            },
            series: [{
                name: '签到微博数',
                color: '#2196f3',
                type: 'column',
                yAxis: 1,
                data: data.count,
                tooltip: {
                    valueSuffix: ''
                }

            }, {
                name: '签到照片数',
                color: '#9c27b0',
                type: 'spline',
                data: data.pics,
                tooltip: {
                    valueSuffix: ''
                }
            }]
        });
    }
    $.post("/chartdata", function(responsedata) {
        var total = 0,
            pictotal = 0;
        var chartData = {
            date: [],
            pics: [],
            count: []
        }
        $(responsedata).each(function(index, val) {
            chartData.date.push(val._id);
            chartData.pics.push(val.value.pics);
            chartData.count.push(val.value.count);
            total += parseInt(val.value.count);
            pictotal += parseInt(val.value.pics);
        });
        drawWeiboCountByMonth(chartData);
        alert('total:' + total+ ';pics:' + pictotal)
        alert('total:' + total / chartData.count.length + ';pics:' + pictotal / chartData.count.length)
    });
});