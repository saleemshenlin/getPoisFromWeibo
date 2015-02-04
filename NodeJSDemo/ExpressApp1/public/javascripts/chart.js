$(function() {

    // 路径配置
    require.config({
        paths: {
            echarts: 'javascripts/build/source' //'/javascripts/build/dist'
        }
    });

    // 使用
    var drawWeiboCountByMonth = function(data){
        require(
            [
                'echarts',
                'echarts/chart/bar',
                'echarts/chart/line' // 使用柱状图就加载bar模块，按需加载
            ],
            function(ec) {
                // 基于准备好的dom，初始化echarts图表
                var myChart = ec.init(document.getElementById('weibo-chart'));

                var option = {
                    color: ['#2196f3', '#9c27b0'],
                    tooltip: {
                        trigger: 'axis'
                    },
                    toolbox: {
                        show: true,
                        feature: {
                            mark: {
                                show: false
                            },
                            dataView: {
                                show: false,
                                readOnly: false
                            },
                            magicType: {
                                show: false,
                                type: ['line', 'bar']
                            },
                            restore: {
                                show: false
                            },
                            saveAsImage: {
                                show: true
                            }
                        }
                    },
                    calculable: true,
                    legend: {
                        data: ['签到微博数', '签到照片数'],
                        textStyle:{
                            fontSize: 16
                        }
                    },
                    xAxis: [{
                        type: 'category',
                        data: data.date,
                        axisLabel: {
                            textStyle:{
                                fontSize: 16
                            }
                        }
                    }],
                    yAxis: [{
                        type: 'value',
                        name: '签到微博数',
                        axisLabel: {
                            formatter: '{value} ',
                            textStyle:{
                                fontSize: 16
                            }
                        }
                    }, {
                        type: 'value',
                        name: '签到照片数',
                        axisLabel: {
                            formatter: '{value} ',
                            textStyle:{
                                fontSize: 16
                            }
                        }
                    }],
                    series: [

                        {
                            name: '签到微博数',
                            type: 'bar',
                            data: data.count
                        }, {
                            name: '签到照片数',
                            type: 'line',
                            yAxisIndex: 1,
                            data: data.pics
                        }
                    ]
                };


                // 为echarts对象加载数据 
                myChart.setOption(option);
            }
        );
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
        //alert('total:' + total+ ';pics:' + pictotal)
        //alert('total:' + total / chartData.count.length + ';pics:' + pictotal / chartData.count.length)
    });

    var drawPieChart = function() {
        $('.weibo-chart').highcharts({
            chart: {
                type: 'pie'
            },
            colors: ['#2196f3', '#9c27b0', '#e91e63', '#00bcd4'],
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
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    depth: 35,
                    dataLabels: {
                        enabled: true,
                        style: {
                            "fontSize": "18px",
                            "fontWeight": "Bold"
                        },
                        format: '{point.name}:{point.y}%'
                    }
                }
            },
            series: [{
                type: 'pie',
                name: 'Browser share',
                data: [
                    ['本市', 67.57],
                    ['国内', 25.42],
                    ['海外', 3.07],
                    ['其他', 3.94]

                ]
            }]
        });
    };
    //drawPieChart();
    var drawLineChart = function() {
        $('.weibo-chart').highcharts({
            chart: {
                type: 'spline'
            },
            colors: ['#2196f3', '#9c27b0', '#e91e63', '#00bcd4'],
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
            xAxis: {
                categories: ['北京', '浙江', '江苏', '福建', '天津', '广东',
                    '湖北', '安徽', '西藏', '海南', '重庆', '陕西', '辽宁', '江西',
                    '四川', '河南', '山东', '湖南', '广西', '黑龙江', '新疆', '吉林',
                    '宁夏', '贵州', '山西', '甘肃', '云南', '内蒙古', '青海', '河北'
                ],
                labels: {
                    style: {
                        'font-size': '18px'
                    }
                }
            },
            yAxis: {
                title: {
                    text: '位置地点签到微博用户数与人口比值',
                    style: {
                        'font-size': '18px'
                    }
                },
                min: 0,
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }]
            },
            tooltip: {
                valueSuffix: '°C'
            },
            legend: {
                enabled: false,
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle',
                borderWidth: 0
            },
            series: [{
                data: [60.41595793, 23.99732882, 22.16758391, 13.53328663,
                    10.2641599, 9.58743981, 7.493307737, 7.216744865, 6.828403226,
                    5.858259188, 5.366396995, 5.074023683, 4.983275966, 4.36641295,
                    4.267690647, 3.204515736, 3.199605316, 2.964204739, 2.668020723,
                    2.586641799, 2.55348403, 2.417860385, 2.142398058, 2.103810954,
                    1.912516457, 1.908094442, 1.836130209, 1.699969817, 1.688372022,
                    1.618555307
                ]
            }]
        });
    };
    //drawLineChart();
});


    // var drawWeiboCountByMonth = function(data) {
    //         $('.weibo-chart').highcharts({
    //             chart: {
    //                 zoomType: 'xy'
    //             },
    //             credits: {
    //                 enabled: false
    //             },
    //             exporting: {
    //                 enabled: true,
    //                 sourceWidth: 1200
    //             },
    //             title: {
    //                 text: ''
    //             },
    //             xAxis: [{
    //                 categories: ['周一', '周二', '周三', '周四', '周五', '周六', '周日'],
    //                 tickInterval: 1,
    //                 labels: {
    //                     style: {
    //                         'font-size': '18px'
    //                     }
    //                 }
    //             }],
    //             yAxis: [{ // Primary yAxis
    //                 labels: {
    //                     format: '{value}',
    //                     style: {
    //                         'font-size': '18px',
    //                         color: '#9c27b0'
    //                     }
    //                 },
    //                 title: {
    //                     text: '签到照片数',
    //                     style: {
    //                         'font-size': '18px',
    //                         color: '#9c27b0'
    //                     }
    //                 }
    //             }, { // Secondary yAxis
    //                 title: {
    //                     text: '签到微博数',
    //                     style: {
    //                         'font-size': '18px',
    //                         color: '#2196f3'
    //                     }
    //                 },
    //                 labels: {
    //                     format: '{value}',
    //                     style: {
    //                         'font-size': '18px',
    //                         color: '#2196f3'
    //                     }
    //                 },
    //                 opposite: true
    //             }],
    //             tooltip: {
    //                 shared: true
    //             },
    //             legend: {
    //                 layout: 'vertical',
    //                 align: 'left',
    //                 x: 120,
    //                 verticalAlign: 'top',
    //                 y: 50,
    //                 floating: true,
    //                 backgroundColor: '#FFFFFF',
    //                 itemStyle: {
    //                     'font-size': '18px'
    //                 }
    //             },
    //             series: [{
    //                 name: '签到微博数',
    //                 color: '#2196f3',
    //                 type: 'column',
    //                 yAxis: 1,
    //                 data: data.count,
    //                 tooltip: {
    //                     valueSuffix: ''
    //                 }

    //             }, {
    //                 name: '签到照片数',
    //                 color: '#9c27b0',
    //                 type: 'spline',
    //                 data: data.pics,
    //                 tooltip: {
    //                     valueSuffix: ''
    //                 }
    //             }]
    //         });
    //     }