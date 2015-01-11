# coding=utf-8
import arcpy
import json
import xml.etree.ElementTree as ET
from arcpy import env

print('start create!!')
def createGeoDatabase():
    # Creating a file geodatabase
    #arcpy.CreateFileGDB_management("C:/paperdata/output", "weibodata.gdb")

    ## Creating a spatial reference object
    spatial_reference = arcpy.SpatialReference('WGS 1984.prj')

    ## Execute CreateFeatureclass
    arcpy.CreateFeatureclass_management('C:/paperdata/output/weibodata.gdb', 'weibo', 'POINT', spatial_reference=spatial_reference)
    print('Create FeatureClass!!')
    # Set environment settings
    env.workspace = 'C:/paperdata/output/weibodata.gdb'

    ## Set local variables
    isFeature = 'weibo'

    ## Execute AddField twice for two new fields(poi)
    #arcpy.AddField_management(isFeature, 'ID', 'TEXT', field_is_nullable = 'NON_NULLABLE')
    #arcpy.AddField_management(isFeature, 'TITLE', 'TEXT', field_is_nullable = 'NON_NULLABLE')
    #arcpy.AddField_management(isFeature, 'CATEGORYNAME', 'TEXT', field_is_nullable = 'NON_NULLABLE')
    #arcpy.AddField_management(isFeature, 'CATEGORY', 'LONG', field_is_nullable = 'NON_NULLABLE')
    #arcpy.AddField_management(isFeature, 'LON', 'DOUBLE', field_is_nullable = 'NON_NULLABLE')
    #arcpy.AddField_management(isFeature, 'LAT', 'DOUBLE', field_is_nullable = 'NON_NULLABLE')

    ## Execute AddField twice for two new fields(weibo)
    arcpy.AddField_management(isFeature, 'ID', 'TEXT', field_is_nullable = 'NON_NULLABLE')
    arcpy.AddField_management(isFeature, 'CREATEAT', 'DATE', field_is_nullable = 'NON_NULLABLE')
    arcpy.AddField_management(isFeature, 'TEXT', 'TEXT', field_is_nullable = 'NON_NULLABLE')
    arcpy.AddField_management(isFeature, 'PICCOUNT', 'SHORT', field_is_nullable = 'NON_NULLABLE')
    arcpy.AddField_management(isFeature, 'LON', 'DOUBLE', field_is_nullable = 'NON_NULLABLE')
    arcpy.AddField_management(isFeature, 'LAT', 'DOUBLE', field_is_nullable = 'NON_NULLABLE')
    arcpy.AddField_management(isFeature, 'USERID', 'TEXT', field_is_nullable = 'NON_NULLABLE')
    arcpy.AddField_management(isFeature, 'USERPROVINCE', 'SHORT', field_is_nullable = 'NON_NULLABLE')
    arcpy.AddField_management(isFeature, 'USERCITY', 'SHORT', field_is_nullable = 'NON_NULLABLE')
    arcpy.AddField_management(isFeature, 'ANNOID', 'TEXT', field_is_nullable = 'NON_NULLABLE')
    arcpy.AddField_management(isFeature, 'ANNOLON', 'DOUBLE', field_is_nullable = 'NON_NULLABLE')
    arcpy.AddField_management(isFeature, 'ANNOLAT', 'DOUBLE', field_is_nullable = 'NON_NULLABLE')
    arcpy.AddField_management(isFeature, 'YEAR', 'SHORT', field_is_nullable = 'NON_NULLABLE')
    arcpy.AddField_management(isFeature, 'ISWEEKEND', 'SHORT', field_is_nullable = 'NON_NULLABLE')


    print('Create Fields!!')

def insertToPOIGDB(poiid,title,lon,lat,category,categoryname):
    # Open an InsertCursor
    #
    c = arcpy.da.InsertCursor('.\weibodata.gdb\poi',('ID','TITLE','LON','LAT','CATEGORY','CATEGORYNAME','SHAPE@XY'))
    # Insert new rows that include the county name and a x,y coordinate 
    #  pair that represents the county center
    #
    point = arcpy.Point(lon, lat)
    row = (poiid,title,lon,lat,category,categoryname,point)
    c.insertRow(row) 
    # Delete cursor object
    #
    del c

def insertToWeiboGDB(id, createat, text, piccount, lon, lat, userid, userprovince, usercity, annoid, annolon, annolat, year, isweekend, num):
    # Open an InsertCursor
    #
    c = arcpy.da.InsertCursor('C:/paperdata/output/weibodata.gdb/weibo'+num,('ID','CREATEAT','TEXT','PICCOUNT','LON','LAT','USERID','USERPROVINCE','USERCITY','ANNOID','ANNOLON','ANNOLAT','YEAR','ISWEEKEND','SHAPE@XY'))
    # Insert new rows that include the county name and a x,y coordinate 
    #  pair that represents the county center
    #
    if (lon == '0') or (lon == 0):
        if float(annolat) > 100 :
            point = arcpy.Point(annolat, annolon)
        else:
            point = arcpy.Point(annolon, annolat)
    else:
        point = arcpy.Point(lon, lat)
    row = (id,createat,text,piccount,lon,lat,userid,userprovince,usercity,annoid,annolon,annolat,year,isweekend,point)
    c.insertRow(row) 
    # Delete cursor object
    #
    del c

def readJsonInsertGDB():
    try:
        fileurl = 'C:\Users\霖\SkyDrive\论文&项目\研究生毕业论文\数据\GetPoisFromWeibo\合并数据\mergeresult_final.json'
        jsonFile = file(fileurl.decode('utf8').encode('gb2312'))
        jaRoot = json.load(jsonFile)
        for jo in jaRoot:
            insertToGDB(jo['poiid'],jo['title'],jo['lon'],jo['lat'],jo['category'],jo['category_name'])
            print jo['title']
        print('read json succeed')
    except Exception, e:
        print e

def readXmlInsertGDB(num):
    try:
        fileXml = 'C:/paperdata/input/listHasData'+num+'.xml'
        tree = ET.parse(fileXml)
        root = tree.getroot()
        count = 0
        for weibo in root.findall('WeiboData'):
            count = count +1
            id = weibo.find('Id').text
            createats = weibo.find('CreatedAt').text.split(' ')
            week = createats[0]
            if week == 'Sat' or week == 'Sun':
                isweekend = 0
            else :
                isweekend = 1
            month = monthToNum(createats[1])
            date =  createats[2]
            time = createats[3]
            year = createats[5]
            createat =  '{0}-{1}-{2} {3}'.format(month, date, year, time)
            if weibo.find('Text').text == None :
                text = 'null'
            else :
                text = (weibo.find('Text').text if len(weibo.find('Text').text)<255 else weibo.find('Text').text[0:250] )
            piccount = weibo.find('PicCount').text
            lon = (weibo.find('Lng').text if weibo.find('Lng').text != 'null' else 0)
            lat = (weibo.find('Lat').text if weibo.find('Lat').text != 'null' else 0)
            userid = weibo.find('UserId').text
            userpro = weibo.find('UserProvince').text
            usercity = weibo.find('UserCity').text
            annoid = weibo.find('AnnotationsId').text
            annolon = (weibo.find('AnnotationsLng').text if weibo.find('AnnotationsLng').text != 'null' else 0)
            annolat = (weibo.find('AnnotationsLat').text if weibo.find('AnnotationsLat').text != 'null' else 0)
            insertToWeiboGDB(id,createat,text,piccount,lon,lat,userid,userpro,usercity,annoid,annolon,annolat,year,isweekend,num)
            print (id,count,num)
        print('read json succeed')
    except Exception, e:
        print e

def monthToNum(x):
    return {
        'Jan': 1,
        'Feb': 2,
        'Mar': 3,
        'Apr': 4,
        'May': 5,
        'Jun': 6,
        'Jul': 7,
        'Aug': 8,
        'Sep': 9,
        'Oct': 10,
        'Nov': 11,
        'Dec': 12,
    }[x]

def startInsert(num):
    createGeoDatabase(num);
    readXmlInsertGDB(num);
    print('Finish Create!!')

def mergeData(a,b):
    arcpy.env.workspace = 'C:/paperdata/output/weibodata.gdb'
    arcpy.Merge_management(['weibo'+a, 'weibo'+b], 'weibo'+a+'and'+b)
    print('Mearge finish')

#createGeoDatabase()
#readXmlInsertGDB("1");

#startInsert('4')
#startInsert('5')
#startInsert('6')
#startInsert('7')
mergeData('12345','67')