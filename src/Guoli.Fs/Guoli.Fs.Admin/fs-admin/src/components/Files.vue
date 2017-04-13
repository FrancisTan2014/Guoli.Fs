<template>
  <section>

    <!-- 工具条 -->
    <el-col :span="24"
            class="toolbar">
      <span class="path-stack">当前目录：
                                <span v-if="pathStack.length === 0">顶级目录</span>
      <span v-for="(p,index) in pathStack"><i class="fa fa-angle-right path-right" v-if="index > 0"></i>{{ p.name }}</span>
      </span>
      <el-button v-if="pathStack.length > 0"
                 @click="handleDirBack">返回</el-button>
      <el-button type="default"
                 v-if="currDirId !== 0 || (currDirId === 0 && isSuperAdminLogin)"
                 v-on:click="handleAddDic">{{ (isSuperAdminLogin && (currDirId === 0 || currDir.IsCommon)) ? '新建公共文件夹' : '新建文件夹' }}</el-button>
      <el-button type="default"
                 v-if="currDirId !== 0 && ((currDir.IsCommon && isSuperAdminLogin) || (!currDir.IsCommon))"
                 v-on:click="handleAddFiles">{{ (isSuperAdminLogin && currDir && currDir.IsCommon) ? '添加公共文件' : '添加文件' }}</el-button>
      <el-button type="primary"
                 v-if="selectedItem && ((selectedItem.IsCommon && isSuperAdminLogin) || (!selectedItem.IsCommon))"
                 v-on:click="handleRename">重命名</el-button>
      <el-button type="danger"
                 v-if="selectedItem && ((selectedItem.IsCommon && isSuperAdminLogin) || (!selectedItem.IsCommon))"
                 v-on:click="handleRemoveItem">删除</el-button>
      <span class="tip">带有 <i class="fa fa-handshake-o share"></i> 图标表示此项对所有人可见</span>
    </el-col>

    <!-- 文件列表 -->
    <el-col :span="24"
            class="content center"
            :loading="isFileLoading">

      <!-- 文件夹 -->
      <el-col title="双击打开"
              :span="3"
              class="folder"
              :class="{ active: isItemSelected(item) }"
              v-for="item in currDirData.dirs"
              :key="item.Id"
              @dblclick.native="fileDbClick(item, true)"
              @click.native="fileClick(item)">
        <i class="folder-img"
           :class="getIconClass('folder')"
           aria-hidden="true"></i>
        <div class="file-info">
          <span class="file-name">{{ item.DirName }}</span>
          <span class="file-time">{{ item.CreateTime | moment("YYYY-MM-DD HH:mm") }}</span>
        </div>
        <i class="fa fa-handshake-o share"
           v-if="item.IsCommon"></i>
      </el-col>

      <!-- 文件 -->
      <el-col title="双击下载"
              :span="3"
              class="folder"
              :class="{ active: isItemSelected(item) }"
              v-for="item in currDirData.files"
              :key="item.Id"
              @dblclick.native="fileDbClick(item, false)"
              @click.native="fileClick(item)">
        <i class="folder-img"
           :class="getIconClass(item.Extension)"
           aria-hidden="true"></i>
        <div class="file-info">
          <span class="file-name">{{ item.FileName + item.Extension }}</span>
          <span class="file-time">{{ item.UploadTime | moment("YYYY-MM-DD HH:mm") }}</span>
        </div>
        <i class="fa fa-handshake-o share"
           v-if="item.IsCommon"></i>
      </el-col>

      <p class="empty-tip"
         v-if="currDirData.dirs.length === 0 && currDirData.files.length === 0">此目录下没有任何内容</p>
    </el-col>

    <!-- 添加/重命名目录 -->
    <el-dialog :title="isAddingDir ? '添加目录' : '重命名'"
               v-model="editDirVisible"
               :close-on-click-modal="false">
      <el-form :model="editDirModel"
               label-width="100px"
               :rules="editDirRules"
               @submit.native.prevent="dirSubmit"
               ref="editDirForm">
        <el-form-item label="目录名称"
                      prop="DirName">
          <el-input v-model="editDirModel.DirName"
                    auto-complete="off"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer"
           class="dialog-footer">
        <el-button @click.native="editDirVisible = false">取消</el-button>
        <el-button type="primary"
                   @click.native="dirSubmit"
                   :loading="editDirLoading">提交</el-button>
      </div>
    </el-dialog>

    <!-- 文件重命名 -->
    <el-dialog title="重命名"
               v-model="fileRenameVisible"
               :close-on-click-modal="false">
      <el-form :model="fileRenameModel"
               label-width="100px"
               :rules="fileRenameRules"
               @submit.native.prevent="fileRenameSubmit"
               ref="fileRenameForm">
        <el-form-item label="文件名称"
                      prop="name">
          <el-input v-model="fileRenameModel.name"
                    auto-complete="off"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer"
           class="dialog-footer">
        <el-button @click.native="fileRenameVisible = false">取消</el-button>
        <el-button type="primary"
                   @click.native="fileRenameSubmit"
                   :loading="fileRenameLoading">提交</el-button>
      </div>
    </el-dialog>

    <!-- 添加文件 -->
    <el-dialog title="添加文件"
               v-model="isUploadVisible">
      <el-upload class="file-upload"
                 ref="upload"
                 :action="uploadPath()"
                 :on-success="uploadSuccess"
                 drag
                 multiple>
        <i class="el-icon-upload"></i>
        <div class="el-upload__text">将文件拖到此处，或<em>点击上传</em></div>
      </el-upload>
    </el-dialog>

    <a :href="downloadFilePath" :download="downloadFileName" v-show="false" ref="download">下载</a>

  </section>
</template>

<script>
import NProgress from 'nprogress'
import moment from 'moment';
import { config, store } from '@/global';
import utils from '../utils';
import dirService from '@/services/dir-service';
import fileService from '@/services/file-service';

let loginUser = store.getLoginUser() || {};
let emptyDirModel = { Id: 0, DirName: '', DepartmentId: loginUser.DepartmentId || 0, ParentId: 0, IsTopestDir: false, IsCommon: false };

export default {
  data() {
    return {
      isSuperAdminLogin: store.isSuperAdminLogin(),
      hasNotFileSelected: true,
      // cache: [],
      pathStack: [],
      isFileLoading: false,

      // 当前目录
      currDirId: 0, // 0表示顶级目录
      currDirName: '顶级目录',
      currDir: null,
      currDirData: { dirs: [], files: [] },
      selectedItem: null,

      // 添加或者重命名目录
      isAddingDir: true,
      editDirVisible: false,
      editDirLoading: false,
      editDirModel: utils.clone(emptyDirModel),
      editDirRules: {
        DirName: [utils.rules.required, utils.rules.getMaxRule(50)]
      },

      // 添加文件
      isUploadVisible: false,

      // 文件重命名
      fileRenameVisible: false,
      fileRenameLoading: false,
      fileRenameModel: { id: 0, name: '', origin: '' },
      fileRenameRules: { name: [utils.rules.required, utils.rules.getMaxRule(50)] },

      // 下载
      downloadFilePath: '',
      downloadFileName: ''
    };
  }, // end data()

  methods: {
    loadFiles: function () {
      NProgress.start();
      this.isFileLoading = true;
      // 这里放弃使用缓存是为了防止服务器有更新时
      // 若客户端不刷新页面则一直获取不到最新信息的情况
      fileService.get(this.currDirId).then(res => {
        NProgress.done();
        this.isFileLoading = false;

        let { data } = res;
        this.currDirData = data;
      });
    },

    getIconClass: extension => {
      var e = extension.toLowerCase();
      return utils.fileIcons[e] || utils.fileIcons['*'];
    },

    // 处理添加目录点击事件
    handleAddDic: function () {
      this.isAddingDir = true;
      this.editDirVisible = true;
      utils.copy(emptyDirModel, this.editDirModel);

      let m = this.editDirModel;
      m.ParentId = this.currDirId;
      if (this.currDirId === 0) {
        // 当前处于顶级目录，所有添加的文件夹均为公共的
        m.IsTopestDir = true;
        m.IsCommon = true;
      } else {
        // 不在顶级目录下时，若当前目录为公共的
        // 则超级管理员向此目录下添加的所有目录都是公共的
        // 若登录的是一般管理员则添加的目录仅对其所属单位可见
        if (this.currDir.IsCommon && this.isSuperAdminLogin) {
          m.IsCommon = true;
        }
      }
    },

    // 处理重命名点击事件
    handleRename: function () {
      let item = this.selectedItem;
      if (!!item) {
        if (!!item.DirName) {
          // 文件夹重命名
          this.isAddingDir = false;
          this.editDirVisible = true;
          utils.copy(item, this.editDirModel);
        } else {
          // 文件重命名
          this.fileRenameVisible = true;
          this.fileRenameModel.id = item.Id;
          this.fileRenameModel.name = item.FileName;
          this.fileRenameModel.origin = item.FileName;
        }
      }
    },

    // 添加/重命名目录表单提交
    dirSubmit: function () {
      let _this = this;
      console.info(_this.isAddingDir);
      _this.$refs.editDirForm.validate(valid => {
        if (valid) {
          if (_this.isAddingDir) {
            _this.addDir();
          } else {
            _this.renameDir();
          }
        }
      });
    },

    // 文件重命名提交
    fileRenameSubmit: function () {
      this.$refs.fileRenameForm.validate(valid => {
        if (valid) {
          let model = this.fileRenameModel;
          let changed = model.name !== model.origin;
          if (changed) {
            NProgress.start();
            fileService.rename(model.id, model.name).then(res => {
              NProgress.done();
              let { code } = res;
              if (code === 201) {
                this.$notify({ type: 'success', title: '提示', message: '文件重命名成功！' });
                this.fileRenameVisible = false;
                this.selectedItem.FileName = model.name;
              } else {
                this.$message({ type: 'error', title: '提示', message: '重命名失败，您可能没有权限对此文件进行操作！' });
              }
            });
          }
        }
      });
    },

    // 添加目录到数据库
    addDir: function () {
      this.editDirLoading = true;
      NProgress.start();

      dirService.add(this.editDirModel).then(res => {
        this.editDirLoading = false;
        NProgress.done();

        let { code, data } = res;
        if (code === 201) {
          this.editDirVisible = false;
          this.$notify({ type: 'success', title: '提示', message: '目录创建成功' });

          this.currDirData.dirs.push(data);
        } else {
          this.$message({ type: 'error', message: '当前目录中已存在相同名称的子目录' });
        }
      });
    },

    // 更新目录名称到数据库
    renameDir: function () {
      this.editDirLoading = true;
      NProgress.start();

      dirService.rename(this.editDirModel.Id, this.editDirModel.DirName)
        .then(res => {
          this.editDirLoading = false;
          NProgress.done();

          let { code } = res;
          if (code === 201) {
            this.editDirVisible = false;
            this.selectedItem.DirName = this.editDirModel.DirName;
            this.$notify({ type: 'success', title: '提示', message: '目录重命名成功' });
          } else {
            this.$message({ type: 'error', message: '名称重复' });
          }
        });
    },

    // 处理删除按钮点击事件
    handleRemoveItem: function () {
      let item = this.selectedItem;
      if (!!item) {
        if (!!item.DirName) {
          this.removeDir();
        } else {
          this.removeFile();
        }
      }
    },

    // 删除文件
    removeFile: function () {
      let item = this.selectedItem;
      this.$confirm('您确定要删除选中文件吗？一旦删除后，它将对所有人不可见！', '警告', { type: 'warning' })
        .then(() => {
          NProgress.start();
          fileService.remove(item.Id).then(res => {
            NProgress.done();
            let { code } = res;
            if (code === 204) {
              this.$notify({ type: 'success', message: '文件删除成功！', title: '提示' });
              this.selectedItem = null;

              // 将目录从当前缓存中移除
              let files = this.currDirData.files;
              let index = files.indexOf(item);
              files.splice(index, 1);
            }
          });
        })
        .catch(() => { /* 取消 */ });
    },

    // 删除目录
    removeDir: function () {
      let item = this.selectedItem;
      this.$confirm('您确定要删除选中文件夹吗？一旦删除后，其与之子文件夹或者子文件均不可见！', '警告', { type: 'warning' })
        .then(() => {
          NProgress.start();
          dirService.remove(item.Id).then(res => {
            NProgress.done();
            let { code } = res;
            if (code === 204) {
              this.$notify({ type: 'success', message: '删除成功！', title: '提示' });
              this.selectedItem = null;

              // 将目录从当前缓存中移除
              let dirs = this.currDirData.dirs;
              let index = dirs.indexOf(item);
              dirs.splice(index, 1);
            }
          });
        })
        .catch(() => { /* 取消 */ });
    },

    // 文件或者文件夹双击事件
    fileDbClick: function (data, isFolder) {
      this.selectedItem = null;
      if (isFolder) {
        // 文件夹被双击，记录当前目录信息
        this.currDirId = data.Id;
        this.currDirName = data.DirName;
        this.currDir = data;

        // 记录路径，以便返回上一层
        this.pathStack.push({ id: this.currDirId, name: this.currDirName });

        // 加载新目录下的子目录及文件
        this.loadFiles();
      } else {
        // 文件被双击
        this.downloadFilePath = config.fileServer + data.Path;
        this.downloadFileName = data.FileName;
        let _this = this;
        setTimeout(function() {
          _this.$refs.download.click();
        }, 50);
      }
    },

    // 文件或者文件夹单击事件
    fileClick: function (data) {
      if (!!this.selectedItem && data.Id === this.selectedItem.Id) {
        this.selectedItem = null;
      } else {
        this.selectedItem = data;
      }
    },

    // 文件或文件夹是否被选中
    isItemSelected: function (item) {
      return !!this.selectedItem && this.selectedItem.Id === item.Id;
    },

    // 返回上级目录
    handleDirBack: function () {
      this.pathStack.pop();

      let arr = this.pathStack;
      let curr = arr[arr.length - 1] || { id: 0, name: '顶级目录' };

      this.selectedItem = null;
      this.currDirId = curr.id;
      this.currDirName = curr.name;

      this.loadFiles();
    },

    // 添加文件
    handleAddFiles: function () {
      let u = this.$refs.upload;
      if (!!u) {
        u.clearFiles();
      }
      this.isUploadVisible = true;
    },

    // 动态获取文件上传地址，因为需要通过url传递动态参数
    uploadPath: function () {
      return `${config.base}/file/upload?app_token=${store.getLoginToken()}&dir=${this.currDirId}`;
    },

    // 文件上传成功
    uploadSuccess: function (response, file, fileList) {
      this.currDirData.files.push(response.data);
      this.$notify({ type: 'success', title: '提示', message: `《${response.data.FileName}》上传成功！` });
    }

  }, // end methods

  mounted() {
    this.loadFiles();
  }
};
</script>

<style scoped lang="scss">
$folderColor: #A3DDFF;
$folderContrastColor: #ED5E07;
$pdfColor: #F14B37;
$lightSilver: #C0CCDA;
$blue: #20A0FF;

.toolbar {
  border-bottom: 1px solid $lightSilver;
}

.path-stack {
  color: $blue;
  margin-right: 15px;
}

.path-right {
  font-size: 14px;
  margin: 0 10px;
}

.tip {
  float: right;
  color: $lightSilver;

  .share {
    color: $folderContrastColor;
  }
}

.empty-tip {
  color: $lightSilver;
  font-size: 20px;
  text-align: center;
  padding: 20px;
}

.back {
  display: inline-block;
  width: 100%;
}

.folder {
  text-align: center;
  padding: 10px;
  cursor: pointer;
  margin-top: 10px;
  position: relative;

  &.active {
    background: $folderColor;

    .folder-img {
      color: #fff;
    }
    .file-info .file-name {
      color: #fff;
    }
    .file-info .file-time {
      color: #fff;
    }
  }

  .file-info {
    // padding: 10px 14px 0 14px;
    span {
      display: inline-block;
      margin-top: 10px;

      &:last-child {
        margin-bottom: 0;
      }
    }

    .file-name {
      font-size: 14px;
      color: #324057;
      width: 100%;
      height: 40px;
      line-height: 20px;
      overflow: hidden;
      text-overflow: ellipsis;
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
    }

    .file-time {
      font-size: 14px;
      color: $lightSilver;
    }
  }

  .share {
    position: absolute;
    top: 20px;
    left: 68px;
    font-size: 20px;
    color: $folderContrastColor;
  }

  &:hover {
    border: 1px solid $folderColor;
    padding: 9px;

    .share {
      top: 19px;
      left: 67px;
    }
  }

  .folder-img {
    width: 100%;
    color: $folderColor;
    font-size: 5rem;
  }
}
</style>
