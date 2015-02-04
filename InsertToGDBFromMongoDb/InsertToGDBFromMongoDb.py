import arcpy
import json
import pymongo
from arcpy import env
print('Hello World')
def insertToWeiboGDB(id, createat, source, piccount, lon, lat, userid, userprovince, usercity, annoid, annolon, annolat):
    # Open an InsertCursor
    #
    c = arcpy.da.InsertCursor('C:/weibodata.gdb/weibo',('ID','CREATEAT','SOURCE','PICCOUNT','LON','LAT','USERID','USERPROVINCE','USERCITY','ANNOID','ANNOLON','ANNOLAT','SHAPE@XY'))
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
    row = (id,createat,source,piccount,lon,lat,userid,userprovince,usercity,annoid,annolon,annolat,point)
    c.insertRow(row) 
    # Delete cursor object
    #
    del c

conn = pymongo.Connection('192.168.199.111',27017)
db = conn.weibo
docs = db.weibo.find(fields={"_id": False}).limit(10000)
for doc in docs:
  #docJson = json.dumps(doc,sort_keys=True,indent=2,ensure_ascii=False)
  weiboId = doc["weibo_id"]
  source = doc["source"]
  picCount = doc["pic_count"]
  lat = doc["geo"]["lat"]
  lon = doc["geo"]["lon"]
  poiId = doc["annotations"]["poiid"]
  poiLat = doc["annotations"]["lat"]
  poiLon = doc["annotations"]["lon"]
  userId = doc["user"]["id"]
  userProvince = doc["user"]["province"]
  userCity = doc["user"]["city"]
  createdAt = doc["created_at"]
  insertToWeiboGDB(weiboId,createdAt,source,picCount,lon,lat,userId,userProvince,userCity,poiId,poiLon,poiLat)
  print(createdAt)



print('end')


#arcpy.CreateFileGDB_management("C:/", "weibodata.gdb")

### Creating a spatial reference object
#spatial_reference = arcpy.SpatialReference('WGS 1984.prj')

### Execute CreateFeatureclass
#arcpy.CreateFeatureclass_management('C:/weibodata.gdb', 'weibo', 'POINT', spatial_reference=spatial_reference)
#print('Create FeatureClass!!')
## Set environment settings
#env.workspace = 'C:/weibodata.gdb'

### Set local variables
#isFeature = 'weibo'

#arcpy.AddField_management(isFeature, 'ID', 'TEXT', field_is_nullable = 'NON_NULLABLE')
#arcpy.AddField_management(isFeature, 'SOURCE', 'TEXT', field_is_nullable = 'NON_NULLABLE')
#arcpy.AddField_management(isFeature, 'CREATEAT', 'DATE', field_is_nullable = 'NON_NULLABLE')
#arcpy.AddField_management(isFeature, 'PICCOUNT', 'SHORT', field_is_nullable = 'NON_NULLABLE')
#arcpy.AddField_management(isFeature, 'LON', 'DOUBLE', field_is_nullable = 'NON_NULLABLE')
#arcpy.AddField_management(isFeature, 'LAT', 'DOUBLE', field_is_nullable = 'NON_NULLABLE')
#arcpy.AddField_management(isFeature, 'USERID', 'TEXT', field_is_nullable = 'NON_NULLABLE')
#arcpy.AddField_management(isFeature, 'USERPROVINCE', 'SHORT', field_is_nullable = 'NON_NULLABLE')
#arcpy.AddField_management(isFeature, 'USERCITY', 'SHORT', field_is_nullable = 'NON_NULLABLE')
#arcpy.AddField_management(isFeature, 'ANNOID', 'TEXT', field_is_nullable = 'NON_NULLABLE')
#arcpy.AddField_management(isFeature, 'ANNOLON', 'DOUBLE', field_is_nullable = 'NON_NULLABLE')
#arcpy.AddField_management(isFeature, 'ANNOLAT', 'DOUBLE', field_is_nullable = 'NON_NULLABLE')