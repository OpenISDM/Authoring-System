﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="HistoricalRepository.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Historical Hazard Data Repository</title>
        <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.1/css/bootstrap.min.css" rel=stylesheet>
        <link href="https://kkbruce.tw/Content/AssetsBS3/examples/starter-template.css" rel=stylesheet> 

        <link rel="stylesheet" href="JS_CSS/jquery-ui.css">
        <link rel="stylesheet" type="text/css" media="screen" href="JS_CSS/ui.jqgrid.css" />
        <link rel="stylesheet" href="JS_CSS/bootstrap-timepicker.min.css">

        <script src="https://code.jquery.com/jquery-1.11.3.min.js"></script>
        
        <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.1/js/bootstrap.min.js"></script>
        
        <!--<script src="bootstrap-without-jquery.min.js"></script>-->
        <script src="JS_CSS/bootstrap-timepicker.min.js"></script>    
        <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
        <script src="JS_CSS/jquery.jqGrid.min.js"></script>
        <script type="text/ecmascript" src="JS_CSS/grid.locale-tw.js"></script>

    <style type="text/css">
        #prodId {
            width: 133px;
        }
        #hazardType {
            width: 140px;
        }
        #errorMsg_div {
            visibility: hidden;
        }
        #datalist_tb {
            border-collapse: collapse;
            margin: 0 auto;
            visibility: hidden;            
        }
        table, .datalisttr, th, td {
            border: 1px solid black;
            padding-left: 15px;
            padding-right: 15px;
            padding-top: 10px;
            padding-bottom: 10px;
            text-align: center;
        }
        .dataSourceLogo {
            border: 1px solid black;
        }
        .ui-datepicker { 
            z-index: 9999 !important; 
        }
        #jqGridDiv {
            margin:0px auto;
            text-align:center;
            width: 1000px;
            height: 1200px;
        }
        #jqGrid {
            visibility: hidden;
        }
        #BackDiv {
            float:left;
            width:50%;
        }
        #uploadBack {
            float:left;
            width:40%;
        }
        #searchBack {
            float: right;
            width:60%;
        }
        #fileup_div {
            float:left;
        }
        #uploadDiv {
            text-align:center;
            
        }
        .one-third_PH {
            width: 33%;
            float: left;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">

            <nav class="navbar navbar-inverse navbar-fixed-top" role=navigation>
                <div class=container id="navbar_con">
                    <div class=navbar-header>
                        <button type=button class="navbar-toggle collapsed" data-toggle=collapse data-target=#navbar aria-expanded=false aria-controls=navbar>
                            <span class=sr-only>Toggle navigation</span>
                            <span class=icon-bar></span> <span class=icon-bar></span>
                            <span class=icon-bar></span>

                        </button>
                        <a class=navbar-brand href=#>Natural Hazard Repository</a>

                    </div>
                    <div id=navbar class="collapse navbar-collapse">
                        <ul class="nav navbar-nav">
                            <li class=active>
                                <a href=#home data-toggle="tab">Home</a>
                                <li>
                                    <a href=#search data-toggle="tab">Search</a>
                                    <li>
                                        <a href=#upload data-toggle="tab">Upload</a>
                                        </ul>

                    </div>

                </div>

            </nav>


            <div class="container">
                <div class="starter-template">
                <div class="tab-content">
                      <div class="tab-pane fade in active" id="home">
                                                  <h1>Index Template</h1>
                        <p class=lead>blablablabla<br>
                             some text or pics here
                        repository repository repository repository</p>
                      </div>
                      <div class="tab-pane" id="search">
                            <h1>Search</h1>
                            <div id="datepicker_div">
                                <input id="startdatepicker" type="text" />
                                <input id="enddatepicker" type="text" />
                            </div>
                            <br/>
                            <select id="hazardType" name="D1">
                                <option>請選擇</option>
                                <option>Earthquake</option>
                                <option>Typhoon</option>
                            </select><input type="button" value="Search" onclick="find();" /><br />
                            <div id="jqGridDiv">
                                <div>
                                    <table id="jqGrid"></table>
                                </div>
                            </div>
                            <div id="jqGridPager">
                            </div>
                            <div id ="datalist_div">
                                <div id="errorMsg_div">ERROR</div>
                                    <table id="datalist_tb">
                                    <tr><td>時間</td><td>經度</td><td>緯度</td><td>深度</td><td>規模</td><td>資料連結</td></tr>
                                        </table>
                            </div>

                      </div>


                      <div class="tab-pane" id="upload">
                        <h1>Upload</h1>
                          
                        <div id="uploadDiv">
                            <h5>日期  </h5><input id="uploaddatepicker" type="text" runat="server"/>
                            <h5>時間  （GMT + 8）</h5>
                            <div class="input-group bootstrap-timepicker timepicker">
                                <input id="uploadtimepicker" type="text" class="form-control input-small" runat="server">
                                <span class="input-group-addon"><i class="glyphicon glyphicon-time"></i></span>
                            </div>
                            <h5>檔案名稱</h5><asp:TextBox ID="uploadName" runat="server"></asp:TextBox>
                            <br />
                            <div class="one-third_PH"></div>
                            <div id="fileup_div">
                            <h5>檔案上傳  </h5>
                                <asp:FileUpload ID="FileUp" runat="server" />
                            </div>
                            <br />
                            <h5>災害類別  </h5><asp:DropDownList ID="DropDownList2" runat="server">
                                <asp:ListItem>Earthquake</asp:ListItem>
                                <asp:ListItem>Typhoon</asp:ListItem>
                            </asp:DropDownList>
                            <br />
                            <br />

                        <asp:Button ID="UploadBtn" runat="server" OnClick="UploadBtn_Click" Text="Upload" />
                        </div>
                          
                      </div>

               </div>
                </div>
            </div>



    <script>
      
    var uri = 'api';

    $(document).ready(function () {
        $("#startdatepicker").datepicker({ "dateFormat": "yy-mm-dd" });
        $("#enddatepicker").datepicker({ "dateFormat": "yy-mm-dd" });
        $("#uploaddatepicker").datepicker({ "dateFormat": "yy-mm-dd" });
        $('#uploadtimepicker').timepicker({ showMeridian: false });
    });

    function timeStringFm(cellvalue, options, rowObject) {
        var date = cellvalue.substr(0, 4) + "年" + cellvalue.substr(4, 2) + "月" + cellvalue.substr(6, 2) + "日 <br />" +
                    cellvalue.substr(8, 2) + "時" + cellvalue.substr(10, 2) + "分";
        return date;
    }

    function dataLinkFm(cellvalue, options, rowObject) {

        return '<a href ="' + cellvalue + '" target="_blank">' + '<img src="http://i.imgur.com/56V6Qxb.png" height="32" width="32"></a>';
    }

    function dataSourceFm(cellvalue, options, rowObject) {
        switch (cellvalue) {
            case 0:
                return '<img src="http://i.imgur.com/qMlE9il.png" height="64" width="64">';
            case 1:
                return '<img src="http://i.imgur.com/ykvvL9w.png" height="64" width="64">';
            case 2:
                return '<img src="http://i.imgur.com/hkf5d1u.png" height="64" width="64">';
            case 3:
                return '<img src="http://i.imgur.com/nTyfXKn.jpg" height="64" width="64">';
            case 4:
                return '<img src="http://i.imgur.com/sGKz90Y.jpg" height="64" width="64">';
            default:
                return null;
        }
    }

    function find() {
      var id = '?startdate=' + $('#startdatepicker').val() + '&enddate=' + $('#enddatepicker').val();
      var type = $('#hazardType').val();
      $('.datalisttr').remove();   
      switch (type) {
          case 'Earthquake':
            $.getJSON(uri + '/' + type + '/' + id)
            .done(function (data) {

                $('#errorMsg_div').css({ "visibility": "hidden" });
                $('#jqGrid').css({ "visibility": "visible" });
                $('#jqGrid').jqGrid({
                    datastr: data,
                    datatype: "jsonstring",
                    colModel: [
                        { label: 'Time', name: 'timeString', width: 120,  formatter: timeStringFm },
                        { label: 'Lat', name: 'lat', width: 60 },
                        { label: 'Lng', name: 'lng', width: 60 },
                        { label: 'Depth', name: 'depth', width: 60 },
                        { label: 'Magnitude', name: 'magnitude', width: 60 },
                        { label: 'Source', name: 'dataSource', width: 60, formatter: dataSourceFm },
                        { label: 'Link', name: 'dataLink', width: 60, formatter: dataLinkFm }
                    ],
                    viewrecords: true,
                    height: 700,
                    rowNum: 300,
                    autowidth: true,
                    pager: "#jqGridPager"
                });
                jQuery('#jqGrid').jqGrid('clearGridData');
                jQuery('#jqGrid').jqGrid('setGridParam', { data: data });
                jQuery('#jqGrid').trigger('reloadGrid');
            })
            .fail(function (jqXHR, textStatus, err) {
                $('#datalist_tb').css({ "visibility": "hidden" });
                $('#errorMsg_div').css({ "visibility": "visible" }).html("ERROR");
            });
            break;
          case 'Typhoon':
            $('#datalist_tb').css({ "visibility": "hidden" });
            $('#errorMsg_div').css({ "visibility": "visible" }).html("TYPHOON");
            break;
        case 3:
            break;
        default:
            break;
      }
    }
    </script>
    </form>
</body>

</html>