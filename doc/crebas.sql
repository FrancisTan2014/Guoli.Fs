/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2012                    */
/* Created on:     2017/5/8 10:23:21                            */
/*==============================================================*/


if exists (select 1
            from  sysobjects
           where  id = object_id('ViewSystemUser')
            and   type = 'V')
   drop view ViewSystemUser
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ViewPersonInfo')
            and   type = 'V')
   drop view ViewPersonInfo
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ViewDepartFiles')
            and   type = 'V')
   drop view ViewDepartFiles
go

if exists (select 1
            from  sysobjects
           where  id = object_id('DbUpdateLog')
            and   type = 'U')
   drop table DbUpdateLog
go

if exists (select 1
            from  sysobjects
           where  id = object_id('DepartFiles')
            and   type = 'U')
   drop table DepartFiles
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Department')
            and   type = 'U')
   drop table Department
go

if exists (select 1
            from  sysobjects
           where  id = object_id('FileDirectory')
            and   type = 'U')
   drop table FileDirectory
go

if exists (select 1
            from  sysobjects
           where  id = object_id('FileInfo')
            and   type = 'U')
   drop table FileInfo
go

if exists (select 1
            from  sysobjects
           where  id = object_id('OperationLog')
            and   type = 'U')
   drop table OperationLog
go

if exists (select 1
            from  sysobjects
           where  id = object_id('PersonInfo')
            and   type = 'U')
   drop table PersonInfo
go

if exists (select 1
            from  sysobjects
           where  id = object_id('SystemDict')
            and   type = 'U')
   drop table SystemDict
go

if exists (select 1
            from  sysobjects
           where  id = object_id('SystemLog')
            and   type = 'U')
   drop table SystemLog
go

if exists (select 1
            from  sysobjects
           where  id = object_id('SystemUser')
            and   type = 'U')
   drop table SystemUser
go

/*==============================================================*/
/* Table: DbUpdateLog                                           */
/*==============================================================*/
create table DbUpdateLog (
   Id                   int                  identity,
   TableName            varchar(50)          not null,
   TargetId             int                  not null,
   OperateType          int                  not null,
   UpdateTime           datetime             not null default getdate(),
   constraint PK_DBUPDATELOG primary key (Id)
)
go

if exists (select 1 from  sys.extended_properties
           where major_id = object_id('DbUpdateLog') and minor_id = 0)
begin 
   declare @CurrentUser sysname 
select @CurrentUser = user_name() 
execute sp_dropextendedproperty 'MS_Description',  
   'user', @CurrentUser, 'table', 'DbUpdateLog' 
 
end 


select @CurrentUser = user_name() 
execute sp_addextendedproperty 'MS_Description',  
   '数据库更新记录表', 
   'user', @CurrentUser, 'table', 'DbUpdateLog'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('DbUpdateLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Id')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'DbUpdateLog', 'column', 'Id'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', @CurrentUser, 'table', 'DbUpdateLog', 'column', 'Id'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('DbUpdateLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'TableName')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'DbUpdateLog', 'column', 'TableName'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '表名',
   'user', @CurrentUser, 'table', 'DbUpdateLog', 'column', 'TableName'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('DbUpdateLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'TargetId')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'DbUpdateLog', 'column', 'TargetId'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '目标Id',
   'user', @CurrentUser, 'table', 'DbUpdateLog', 'column', 'TargetId'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('DbUpdateLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'OperateType')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'DbUpdateLog', 'column', 'OperateType'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '操作类型（1 插入； 2 更新； 3 删除）',
   'user', @CurrentUser, 'table', 'DbUpdateLog', 'column', 'OperateType'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('DbUpdateLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'UpdateTime')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'DbUpdateLog', 'column', 'UpdateTime'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '更新时间',
   'user', @CurrentUser, 'table', 'DbUpdateLog', 'column', 'UpdateTime'
go

/*==============================================================*/
/* Table: DepartFiles                                           */
/*==============================================================*/
create table DepartFiles (
   Id                   int                  identity,
   FileName             nvarchar(50)         not null,
   FileInfoId           int                  not null,
   FileDirectoryId      int                  not null,
   DepartmentId         int                  not null,
   IsCommon             bit                  not null,
   SystemUserId         int                  not null,
   LastModifyTime       datetime             not null default getdate(),
   IsDeleted            bit                  not null default 0,
   constraint PK_DEPARTFILES primary key (Id)
)
go

if exists (select 1 from  sys.extended_properties
           where major_id = object_id('DepartFiles') and minor_id = 0)
begin 
   declare @CurrentUser sysname 
select @CurrentUser = user_name() 
execute sp_dropextendedproperty 'MS_Description',  
   'user', @CurrentUser, 'table', 'DepartFiles' 
 
end 


select @CurrentUser = user_name() 
execute sp_addextendedproperty 'MS_Description',  
   '部门文件表', 
   'user', @CurrentUser, 'table', 'DepartFiles'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('DepartFiles')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Id')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'Id'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'Id'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('DepartFiles')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'FileName')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'FileName'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文件名称',
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'FileName'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('DepartFiles')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'FileInfoId')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'FileInfoId'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文件信息Id，关联文件信息表的主键',
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'FileInfoId'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('DepartFiles')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'FileDirectoryId')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'FileDirectoryId'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '部门文件目录Id，关联文件目录表主键',
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'FileDirectoryId'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('DepartFiles')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'DepartmentId')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'DepartmentId'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '部门Id（文件所属部门，若为公共文件则此字段为0）',
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'DepartmentId'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('DepartFiles')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'IsCommon')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'IsCommon'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '是否公共文件（由超级管理员上传，并且所有人都能看到的文件）',
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'IsCommon'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('DepartFiles')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'SystemUserId')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'SystemUserId'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '上传者Id，关联系统用户表的主键',
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'SystemUserId'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('DepartFiles')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'LastModifyTime')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'LastModifyTime'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '最后一次修改时间',
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'LastModifyTime'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('DepartFiles')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'IsDeleted')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'IsDeleted'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '删除标识',
   'user', @CurrentUser, 'table', 'DepartFiles', 'column', 'IsDeleted'
go

/*==============================================================*/
/* Table: Department                                            */
/*==============================================================*/
create table Department (
   Id                   int                  identity,
   Name                 nvarchar(50)         not null,
   ParentId             int                  not null default 0,
   AddTime              datetime             not null default getdate(),
   IsDeleted            bit                  not null default 0,
   constraint PK_DEPARTMENT primary key (Id)
)
go

if exists (select 1 from  sys.extended_properties
           where major_id = object_id('Department') and minor_id = 0)
begin 
   declare @CurrentUser sysname 
select @CurrentUser = user_name() 
execute sp_dropextendedproperty 'MS_Description',  
   'user', @CurrentUser, 'table', 'Department' 
 
end 


select @CurrentUser = user_name() 
execute sp_addextendedproperty 'MS_Description',  
   '部门信息表', 
   'user', @CurrentUser, 'table', 'Department'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('Department')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Id')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'Department', 'column', 'Id'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', @CurrentUser, 'table', 'Department', 'column', 'Id'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('Department')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Name')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'Department', 'column', 'Name'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '部门名称',
   'user', @CurrentUser, 'table', 'Department', 'column', 'Name'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('Department')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'ParentId')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'Department', 'column', 'ParentId'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '父级Id',
   'user', @CurrentUser, 'table', 'Department', 'column', 'ParentId'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('Department')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'AddTime')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'Department', 'column', 'AddTime'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '添加时间',
   'user', @CurrentUser, 'table', 'Department', 'column', 'AddTime'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('Department')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'IsDeleted')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'Department', 'column', 'IsDeleted'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '删除标识',
   'user', @CurrentUser, 'table', 'Department', 'column', 'IsDeleted'
go

/*==============================================================*/
/* Table: FileDirectory                                         */
/*==============================================================*/
create table FileDirectory (
   Id                   int                  identity,
   DirName              nvarchar(50)         not null,
   DepartmentId         int                  not null,
   ParentId             int                  not null,
   IsTopestDir          bit                  not null,
   IsCommon             bit                  not null,
   CreateTime           datetime             not null default getdate(),
   LastModifyTime       datetime             not null default getdate(),
   CreatorId            int                  not null,
   IsDeleted            bit                  not null default 0,
   constraint PK_FILEDIRECTORY primary key (Id)
)
go

if exists (select 1 from  sys.extended_properties
           where major_id = object_id('FileDirectory') and minor_id = 0)
begin 
   declare @CurrentUser sysname 
select @CurrentUser = user_name() 
execute sp_dropextendedproperty 'MS_Description',  
   'user', @CurrentUser, 'table', 'FileDirectory' 
 
end 


select @CurrentUser = user_name() 
execute sp_addextendedproperty 'MS_Description',  
   '文件目录表', 
   'user', @CurrentUser, 'table', 'FileDirectory'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileDirectory')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Id')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'Id'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'Id'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileDirectory')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'DirName')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'DirName'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '目录名称',
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'DirName'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileDirectory')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'DepartmentId')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'DepartmentId'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '部门Id，关联部门信息表主键',
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'DepartmentId'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileDirectory')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'ParentId')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'ParentId'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '上级目录Id，关联本表主键',
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'ParentId'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileDirectory')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'IsTopestDir')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'IsTopestDir'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '是否是顶级目录',
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'IsTopestDir'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileDirectory')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'IsCommon')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'IsCommon'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '是否公共目录（由超级管理员创建，所有人都能看到的目录）',
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'IsCommon'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileDirectory')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'CreateTime')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'CreateTime'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'CreateTime'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileDirectory')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'LastModifyTime')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'LastModifyTime'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '上次修改时间',
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'LastModifyTime'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileDirectory')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'CreatorId')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'CreatorId'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建者管理员Id，关联系统用户表主键',
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'CreatorId'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileDirectory')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'IsDeleted')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'IsDeleted'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '删除标识',
   'user', @CurrentUser, 'table', 'FileDirectory', 'column', 'IsDeleted'
go

/*==============================================================*/
/* Table: FileInfo                                              */
/*==============================================================*/
create table FileInfo (
   Id                   int                  identity,
   Extension            varchar(50)          not null,
   Size                 bigint               not null,
   Path                 varchar(200)         not null,
   HashCode             varchar(100)         not null,
   UploadTime           datetime             not null default getdate(),
   IsDeleted            bit                  not null default 0,
   constraint PK_FILEINFO primary key (Id)
)
go

if exists (select 1 from  sys.extended_properties
           where major_id = object_id('FileInfo') and minor_id = 0)
begin 
   declare @CurrentUser sysname 
select @CurrentUser = user_name() 
execute sp_dropextendedproperty 'MS_Description',  
   'user', @CurrentUser, 'table', 'FileInfo' 
 
end 


select @CurrentUser = user_name() 
execute sp_addextendedproperty 'MS_Description',  
   '描述文件基本信息（如：名称、类型、大小、路径等）', 
   'user', @CurrentUser, 'table', 'FileInfo'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileInfo')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Id')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileInfo', 'column', 'Id'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', @CurrentUser, 'table', 'FileInfo', 'column', 'Id'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileInfo')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Extension')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileInfo', 'column', 'Extension'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文件后缀名，由以''.''开头的小写字母组成，如：.txt',
   'user', @CurrentUser, 'table', 'FileInfo', 'column', 'Extension'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileInfo')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Size')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileInfo', 'column', 'Size'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文件大小，以字节为单位',
   'user', @CurrentUser, 'table', 'FileInfo', 'column', 'Size'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileInfo')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Path')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileInfo', 'column', 'Path'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文件地址',
   'user', @CurrentUser, 'table', 'FileInfo', 'column', 'Path'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileInfo')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'HashCode')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileInfo', 'column', 'HashCode'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '哈希值',
   'user', @CurrentUser, 'table', 'FileInfo', 'column', 'HashCode'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileInfo')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'UploadTime')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileInfo', 'column', 'UploadTime'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '上传时间',
   'user', @CurrentUser, 'table', 'FileInfo', 'column', 'UploadTime'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('FileInfo')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'IsDeleted')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'FileInfo', 'column', 'IsDeleted'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '删除标识',
   'user', @CurrentUser, 'table', 'FileInfo', 'column', 'IsDeleted'
go

/*==============================================================*/
/* Table: OperationLog                                          */
/*==============================================================*/
create table OperationLog (
   Id                   int                  identity,
   SystemUserId         int                  not null,
   OperationType        int                  not null,
   OperationTime        datetime             not null default getdate(),
   OperationDesc        nvarchar(1000)       not null,
   TargetTableName      varchar(50)          not null,
   TargetId             int                  not null,
   constraint PK_OPERATIONLOG primary key (Id)
)
go

if exists (select 1 from  sys.extended_properties
           where major_id = object_id('OperationLog') and minor_id = 0)
begin 
   declare @CurrentUser sysname 
select @CurrentUser = user_name() 
execute sp_dropextendedproperty 'MS_Description',  
   'user', @CurrentUser, 'table', 'OperationLog' 
 
end 


select @CurrentUser = user_name() 
execute sp_addextendedproperty 'MS_Description',  
   '操作日志表，记录管理员在后台进行的操作日志信息', 
   'user', @CurrentUser, 'table', 'OperationLog'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('OperationLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Id')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'OperationLog', 'column', 'Id'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', @CurrentUser, 'table', 'OperationLog', 'column', 'Id'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('OperationLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'SystemUserId')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'OperationLog', 'column', 'SystemUserId'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '操作人Id，关联系统用户表的主键',
   'user', @CurrentUser, 'table', 'OperationLog', 'column', 'SystemUserId'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('OperationLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'OperationType')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'OperationLog', 'column', 'OperationType'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '操作类型（1 新增； 2 修改； 3 删除；）',
   'user', @CurrentUser, 'table', 'OperationLog', 'column', 'OperationType'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('OperationLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'OperationTime')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'OperationLog', 'column', 'OperationTime'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '操作时间',
   'user', @CurrentUser, 'table', 'OperationLog', 'column', 'OperationTime'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('OperationLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'OperationDesc')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'OperationLog', 'column', 'OperationDesc'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '描述信息',
   'user', @CurrentUser, 'table', 'OperationLog', 'column', 'OperationDesc'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('OperationLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'TargetTableName')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'OperationLog', 'column', 'TargetTableName'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '被操作表名称',
   'user', @CurrentUser, 'table', 'OperationLog', 'column', 'TargetTableName'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('OperationLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'TargetId')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'OperationLog', 'column', 'TargetId'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '被操作数据Id',
   'user', @CurrentUser, 'table', 'OperationLog', 'column', 'TargetId'
go

/*==============================================================*/
/* Table: PersonInfo                                            */
/*==============================================================*/
create table PersonInfo (
   Id                   int                  identity,
   Name                 nvarchar(20)         not null,
   WorkNo               varchar(50)          not null,
   Gender               int                  not null,
   HeadPortraitPath     varchar(100)         not null default '',
   DepartmentId         int                  not null,
   AddTime              datetime             not null default getdate(),
   IsDeleted            bit                  not null default 0,
   constraint PK_PERSONINFO primary key (Id)
)
go

if exists (select 1 from  sys.extended_properties
           where major_id = object_id('PersonInfo') and minor_id = 0)
begin 
   declare @CurrentUser sysname 
select @CurrentUser = user_name() 
execute sp_dropextendedproperty 'MS_Description',  
   'user', @CurrentUser, 'table', 'PersonInfo' 
 
end 


select @CurrentUser = user_name() 
execute sp_addextendedproperty 'MS_Description',  
   '描述人员基本信息，如：姓名、性别、生日、所属单位等', 
   'user', @CurrentUser, 'table', 'PersonInfo'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('PersonInfo')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Id')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'Id'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'Id'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('PersonInfo')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Name')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'Name'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '姓名',
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'Name'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('PersonInfo')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'WorkNo')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'WorkNo'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '工号',
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'WorkNo'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('PersonInfo')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Gender')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'Gender'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '性别（-1 未知； 0 男； 1 女）',
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'Gender'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('PersonInfo')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'HeadPortraitPath')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'HeadPortraitPath'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '头像图片地址',
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'HeadPortraitPath'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('PersonInfo')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'DepartmentId')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'DepartmentId'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '部门Id，关联部门信息表',
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'DepartmentId'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('PersonInfo')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'AddTime')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'AddTime'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '添加时间',
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'AddTime'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('PersonInfo')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'IsDeleted')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'IsDeleted'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '删除标识',
   'user', @CurrentUser, 'table', 'PersonInfo', 'column', 'IsDeleted'
go

/*==============================================================*/
/* Table: SystemDict                                            */
/*==============================================================*/
create table SystemDict (
   Id                   int                  identity,
   "Key"                nvarchar(1000)       not null,
   Value                nvarchar(max)        not null,
   constraint PK_SYSTEMDICT primary key (Id)
)
go

if exists (select 1 from  sys.extended_properties
           where major_id = object_id('SystemDict') and minor_id = 0)
begin 
   declare @CurrentUser sysname 
select @CurrentUser = user_name() 
execute sp_dropextendedproperty 'MS_Description',  
   'user', @CurrentUser, 'table', 'SystemDict' 
 
end 


select @CurrentUser = user_name() 
execute sp_addextendedproperty 'MS_Description',  
   '系统字典表（存储一些用键值对描述的信息）', 
   'user', @CurrentUser, 'table', 'SystemDict'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('SystemDict')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Id')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'SystemDict', 'column', 'Id'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', @CurrentUser, 'table', 'SystemDict', 'column', 'Id'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('SystemDict')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Key')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'SystemDict', 'column', 'Key'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '键',
   'user', @CurrentUser, 'table', 'SystemDict', 'column', 'Key'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('SystemDict')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Value')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'SystemDict', 'column', 'Value'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '值',
   'user', @CurrentUser, 'table', 'SystemDict', 'column', 'Value'
go

/*==============================================================*/
/* Table: SystemLog                                             */
/*==============================================================*/
create table SystemLog (
   Id                   int                  not null,
   LogTime              datetime             not null default getdate(),
   Thread               varchar(255)         not null,
   LogLevel             varchar(50)          not null,
   Logger               varchar(255)         not null,
   LogMsg               nvarchar(4000)       not null default '',
   Exception            nvarchar(2000)       null default '',
   constraint PK_SYSTEMLOG primary key (Id)
)
go

if exists (select 1 from  sys.extended_properties
           where major_id = object_id('SystemLog') and minor_id = 0)
begin 
   declare @CurrentUser sysname 
select @CurrentUser = user_name() 
execute sp_dropextendedproperty 'MS_Description',  
   'user', @CurrentUser, 'table', 'SystemLog' 
 
end 


select @CurrentUser = user_name() 
execute sp_addextendedproperty 'MS_Description',  
   '系统日志表', 
   'user', @CurrentUser, 'table', 'SystemLog'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('SystemLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Id')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'SystemLog', 'column', 'Id'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', @CurrentUser, 'table', 'SystemLog', 'column', 'Id'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('SystemLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'LogTime')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'SystemLog', 'column', 'LogTime'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '记录时间',
   'user', @CurrentUser, 'table', 'SystemLog', 'column', 'LogTime'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('SystemLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Thread')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'SystemLog', 'column', 'Thread'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '线程',
   'user', @CurrentUser, 'table', 'SystemLog', 'column', 'Thread'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('SystemLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'LogLevel')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'SystemLog', 'column', 'LogLevel'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '日志级别',
   'user', @CurrentUser, 'table', 'SystemLog', 'column', 'LogLevel'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('SystemLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Logger')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'SystemLog', 'column', 'Logger'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Logger',
   'user', @CurrentUser, 'table', 'SystemLog', 'column', 'Logger'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('SystemLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'LogMsg')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'SystemLog', 'column', 'LogMsg'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '日志信息',
   'user', @CurrentUser, 'table', 'SystemLog', 'column', 'LogMsg'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('SystemLog')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Exception')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'SystemLog', 'column', 'Exception'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '异常信息',
   'user', @CurrentUser, 'table', 'SystemLog', 'column', 'Exception'
go

/*==============================================================*/
/* Table: SystemUser                                            */
/*==============================================================*/
create table SystemUser (
   Id                   int                  identity,
   PersonInfoId         int                  not null,
   Username             varchar(20)          not null,
   Password             varchar(32)          not null,
   UserType             int                  not null,
   IsDeleted            bit                  not null default 0,
   constraint PK_SYSTEMUSER primary key (Id)
)
go

if exists (select 1 from  sys.extended_properties
           where major_id = object_id('SystemUser') and minor_id = 0)
begin 
   declare @CurrentUser sysname 
select @CurrentUser = user_name() 
execute sp_dropextendedproperty 'MS_Description',  
   'user', @CurrentUser, 'table', 'SystemUser' 
 
end 


select @CurrentUser = user_name() 
execute sp_addextendedproperty 'MS_Description',  
   '系统用户表', 
   'user', @CurrentUser, 'table', 'SystemUser'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('SystemUser')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Id')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'SystemUser', 'column', 'Id'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', @CurrentUser, 'table', 'SystemUser', 'column', 'Id'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('SystemUser')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'PersonInfoId')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'SystemUser', 'column', 'PersonInfoId'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '人员信息Id，关联人员信息表的主键',
   'user', @CurrentUser, 'table', 'SystemUser', 'column', 'PersonInfoId'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('SystemUser')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Password')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'SystemUser', 'column', 'Password'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '密码，32位md5字符串',
   'user', @CurrentUser, 'table', 'SystemUser', 'column', 'Password'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('SystemUser')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'UserType')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'SystemUser', 'column', 'UserType'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '用户类型（0 超级管理员，拥有所有权限； 1 普通管理员，拥有管理本部门文件的权限；2 普通用户，权拥有在客户端查看文件的权限）',
   'user', @CurrentUser, 'table', 'SystemUser', 'column', 'UserType'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('SystemUser')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'IsDeleted')
)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'table', 'SystemUser', 'column', 'IsDeleted'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '删除标识',
   'user', @CurrentUser, 'table', 'SystemUser', 'column', 'IsDeleted'
go

/*==============================================================*/
/* View: ViewDepartFiles                                        */
/*==============================================================*/
create view ViewDepartFiles as
SELECT
D.Id,D.[FileName],D.FileInfoId,D.FileDirectoryId,D.DepartmentId,D.IsCommon,D.SystemUserId,D.LastModifyTime,D.IsDeleted,
F.Extension,F.Size,F.[Path],F.HashCode,F.UploadTime,F.IsDeleted AS FileIsDeleted
FROM DepartFiles D
LEFT JOIN FileInfo F
ON D.FileInfoId=F.Id
go

if exists (select 1 from  sys.extended_properties
           where major_id = object_id('ViewDepartFiles') and minor_id = 0)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'view', 'ViewDepartFiles'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '部门文件视图',
   'user', @CurrentUser, 'view', 'ViewDepartFiles'
go

/*==============================================================*/
/* View: ViewPersonInfo                                         */
/*==============================================================*/
create view ViewPersonInfo as
SELECT
P.Id,P.[Name],P.WorkNo,P.Gender,P.HeadPortraitPath,P.DepartmentId,P.AddTime,P.IsDeleted,
D.[Name] AS DepartmentName,D.ParentId AS ParentDepartmentId,
S.Username,S.UserType,
'' AS [Password]
FROM PersonInfo P
LEFT JOIN Department D ON P.DepartmentId=D.Id
LEFT JOIN SystemUser S ON P.Id=S.PersonInfoId
go

if exists (select 1 from  sys.extended_properties
           where major_id = object_id('ViewPersonInfo') and minor_id = 0)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'view', 'ViewPersonInfo'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '人员信息视图',
   'user', @CurrentUser, 'view', 'ViewPersonInfo'
go

/*==============================================================*/
/* View: ViewSystemUser                                         */
/*==============================================================*/
create view ViewSystemUser as
SELECT
S.Id,S.PersonInfoId,S.Username,S.[Password],S.UserType,S.IsDeleted,
P.[Name],P.WorkNo,P.Gender,P.HeadPortraitPath,P.DepartmentId,P.AddTime,P.IsDeleted AS PersonInfoDeleted
FROM SystemUser S
LEFT JOIN PersonInfo P ON S.PersonInfoId=P.Id
go

if exists (select 1 from  sys.extended_properties
           where major_id = object_id('ViewSystemUser') and minor_id = 0)
begin
   declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_dropextendedproperty 'MS_Description', 
   'user', @CurrentUser, 'view', 'ViewSystemUser'

end


select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '系统用户信息视图',
   'user', @CurrentUser, 'view', 'ViewSystemUser'
go

