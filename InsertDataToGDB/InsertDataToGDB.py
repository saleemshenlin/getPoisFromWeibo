import arcpy
from arcpy import env
print('Start Create!!')
arcpy.CreateFileGDB_management(".", "weibodata.gdb")
print('Finish Create!!')