<template>
  <section>

    <!-- 搜索工具条 -->
    <el-col :span="24"
            class="toolbar">
      <el-form :inline="true"
               :model="filters"
               @submit.native.prevent="loadPerson">
        <el-form-item>
          <el-input v-model="filters.name"
                    placeholder="姓名 | 工号"></el-input>
        </el-form-item>
        <el-form-item v-if="isSuperAdminLogin">
          <el-select v-model="filters.departId"
                     placeholder="所有单位">
            <el-option v-for="depart in departs"
                       :label="depart.Name"
                       :value="depart.Id"
                       :key="depart.Name">
            </el-option>
          </el-select>
        </el-form-item>
        <el-form-item v-if="isSuperAdminLogin">
          <el-select v-model="filters.userType"
                     placeholder="所有账号类型">
            <el-option v-for="item in types"
                       :label="item.name"
                       :value="item.value"
                       :key="item.name">
              <span class="fl">{{ item.name }}</span>
              <span class="fr desc">{{ item.desc }}</span>
            </el-option>
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary"
                     v-on:click="loadPerson">查询</el-button>
        </el-form-item>
        <el-form-item>
          <el-button type="primary"
                     @click="handleAdd">新增</el-button>
        </el-form-item>
      </el-form>
    </el-col>

    <!-- 列表 -->
    <el-table :data="list"
              highlight-current-row
              v-loading="isDataLoading"
              width="100%">
      <el-table-column type="selection"
                       width="55"></el-table-column>
      <el-table-column type="index"
                       width="80"></el-table-column>
      <el-table-column prop="Name"
                       label="姓名"
                       width="180"
                       sortable></el-table-column>
      <el-table-column prop="WorkNo"
                       label="工号"
                       width="180"
                       sortable></el-table-column>
      <el-table-column prop="Gender"
                       label="性别"
                       width="120"
                       sortable
                       :formatter="genderFormatter"></el-table-column>
      <el-table-column prop="DepartmentName"
                       label="所属"
                       width="180"
                       sortable></el-table-column>
      <el-table-column prop="Username"
                       label="登录账号"
                       width="120"
                       sortable></el-table-column>
      <el-table-column prop="UserType"
                       label="账号类型"
                       width="120"
                       sortable
                       :formatter="typeFormatter"></el-table-column>
      <el-table-column prop="AddTime"
                       label="添加时间"
                       min-width="240"
                       :formatter="timeFormatter"
                       sortable></el-table-column>
      <el-table-column label="操作"
                       width="180">
        <template scope="scope">
          <el-button size="small"
                     @click="handleEdit(scope.$index, scope.row)">修改</el-button>
          <el-button type="danger"
                     size="small"
                     @click="handleDel(scope.$index, scope.row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 分页工具条 -->
    <el-col :span="24"
            class="toolbar">
      共
      <span>{{ total }}</span> 条， 每页显示
      <el-select v-model="size"
                 size="small"
                 style="width: 70px;"
                 @change="loadPerson">
        <el-option v-for="item in sizes"
                   :value="item"
                   :label="item"
                   :key="item"></el-option>
      </el-select>
      条
      <el-pagination layout="prev, pager, next"
                     @current-change="handlePageChanged"
                     :page-size="size"
                     :total="total"
                     class="fr"></el-pagination>
    </el-col>

    <!-- 编辑界面 -->
    <el-dialog :title="isEditing ? '修改' : '新增'"
               v-model="editFormVisible"
               :close-on-click-modal="false">
      <el-form :model="editModel"
               label-width="100px"
               :rules="editFormRules"
               @submit.native.prevent="editSubmit"
               ref="editForm">
        <el-form-item label="姓名"
                      prop="Name">
          <el-input v-model="editModel.Name"
                    auto-complete="off"></el-input>
        </el-form-item>
        <el-form-item label="性别"
                      prop="Gender">
          <el-radio-group v-model="editModel.Gender">
            <el-radio class="radio"
                      :label="0">男</el-radio>
            <el-radio class="radio"
                      :label="1">女</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="工号"
                      prop="WorkNo">
          <el-input v-model="editModel.WorkNo"
                    @keyup.native="handleWorkNoChange"
                    auto-complete="off"></el-input>
        </el-form-item>
        <el-form-item label="所属单位"
                      prop="DepartmentId"
                      v-if="isSuperAdminLogin">
          <el-select v-model="editModel.DepartmentId"
                     placeholder="请选择单位">
            <el-option v-for="depart in departs"
                       :label="depart.Name"
                       :value="depart.Id"
                       :key="depart.Name">
            </el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="账号类型"
                      prop="UserType"
                      v-if="isSuperAdminLogin">
          <el-select v-model="editModel.UserType"
                     placeholder="请选择账号类型">
            <el-option v-for="item in types"
                       :label="item.name"
                       :value="item.value"
                       :key="item.name">
              <span class="fl">{{ item.name }}</span>
              <span class="fr desc">{{ item.desc }}</span>
            </el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="登录账号"
                      prop="Username">
          <el-input v-model="editModel.Username"
                    auto-complete="off"></el-input>
        </el-form-item>
        <el-form-item v-if="!isEditing"
                      label="登录密码">
          <el-input v-model="editModel.Password"
                    auto-complete="off"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer"
           class="dialog-footer">
        <el-button @click.native="editFormVisible = false">取消</el-button>
        <el-button type="primary"
                   @click.native="editSubmit"
                   :loading="editLoading">提交</el-button>
      </div>
    </el-dialog>

  </section>
</template>

<script>
import NProgress from 'nprogress';
import moment from 'moment';
import personService from '../services/person-service';
import departService from '../services/department-service';
import utils from '../utils';
import { store } from '../global';

let modelTemplate = { Name: '', WorkNo: '', Gender: 0, HeadPortraitPath: '', DepartmentId: undefined, UserType: undefined, Username: '', Password: '123456' };

export default {
  data() {
    return {
      // 权限控制
      isSuperAdminLogin: store.isSuperAdminLogin(),

      // 列表展示
      list: [],
      isDataLoading: false,
      page: 1,
      sizes: [10, 20, 50, 100],
      size: 10,
      total: 0,
      filters: {
        name: '',
        departId: undefined,
        userType: undefined
      },

      departs: [],
      types: [
        { name: '所有账户类型', value: -1 },
        { name: '超级管理员', value: 0, desc: '具有最高权限' },
        { name: '后台管理员', value: 1, desc: '具有上传文件的权限' },
        { name: '一般职工', value: 2, desc: '仅具有查看文件的权限' },
      ],

      // 表单录入
      isEditing: false,
      editFormVisible: false,
      editLoading: false,
      editModel: utils.clone(modelTemplate),
      editFormRules: {
        Name: [utils.rules.required, utils.rules.getMaxRule(15)],
        WorkNo: [utils.rules.required, utils.rules.getMaxRule(15)],
        DepartmentId: [utils.rules.selectionRequired(1)],
        UserType: [utils.rules.selectionRequired(0)],
        Username: [utils.rules.required, utils.rules.getMaxRule(16)]
      }
    };
  }, // end data()

  methods: {
    // 加载人员信息列表
    loadPerson: function () {
      this.isDataLoading = true;
      NProgress.start();

      let params = {
        page: this.page,
        size: this.size,
        conditions: {
          name: this.filters.name,
          departId: this.filters.departId,
          userType: this.filters.userType === undefined ? -1 : this.filters.userType
        }
      };

      personService.getPage(params).then(res => {
        this.isDataLoading = false;
        NProgress.done();

        this.list = res.data.list;
        this.total = res.data.total;
      });
    },

    // 加载部门列表
    loadDeparts: function () {
      departService.getAll().then(res => {
        res.data.splice(0, 0, { Name: '所有单位', Id: -1 });
        this.departs = res.data;
      });
    },

    // 工号改变事件
    handleWorkNoChange: function () {
      if (!this.isEditing) {
        // 新增用户时，默认让登录账户与工号保持一致
        this.editModel.Username = this.editModel.WorkNo;
      }
    },

    // 分页页码改变事件
    handlePageChanged: function () {
      this.loadPerson();
    },

    // 新增按钮点击事件
    handleAdd: function () {
      this.isEditing = false;
      this.editFormVisible = true;
      utils.copy(modelTemplate, this.editModel);
    },

    // 编辑按钮点击事件
    handleEdit: function (index, row) {
      this.isEditing = true;
      this.editFormVisible = true;
      utils.copy(row, this.editModel);
    },

    // 删除按钮点击事件
    handleDel: function (index, row) {
      this.$confirm('您正在执行删除操作，确定删除吗？', '警告', { type: 'warning' }).then(() => {
        this.toggleLoading();
        personService.delete(row.Id).then(res => {
          this.toggleLoading();
          let { code, msg } = res;
          if (code === 204) {
            this.loadPerson();
            this.$notify({ title: '提示', type: 'success', message: '删除成功' });
          } else {
            this.$notify({ title: '提示', type: 'error', message: '删除失败，您可能没有权限执行此操作，请联系管理员' });
          }
        });

      }).catch(() => { /* 取消 */ });
    },

    // loading效果的开启与关闭
    toggleLoading: function () {
      if (this.editLoading) {
        this.editLoading = false;
        NProgress.done();
      } else {
        this.editLoading = true;
        NProgress.start();
      }
    },

    // 处理表格中的性别展示
    genderFormatter: function (row, col) {
      return row.Gender === 0 ? '男' : '女';
    },

    // 处理表格中的账号类型展示
    typeFormatter: function (row, col) {
      switch (row.UserType) {
        case 0: return '超级管理员';
        case 1: return '后台管理员';
        case 2: return '一般职工';
        default: return '';
      }
    },

    // 处理表格中的时间显示
    timeFormatter: function (row, col) {
      return moment(row.AddTime).format('YYYY-MM-DD HH:mm')
    },

    // 确认提交提示
    submitConfirm: function () {
      let msg = '';
      if (this.editModel.UserType === 0) {
        msg = '您确定要添加一个拥有添加、修改、删除所有账户，上传、修改、删除所有文件等权限的超级管理员账户吗？';
      } else if (this.editModel.UserType === 1) {
        msg = '您确定要添加一个拥有添加、修改、删除一般员工账户，上传、修改、删除其所属单位的文件等权限的一般后台管理员账户吗？';
      } else {
        msg = '您确定要添加一个仅拥有登录客户端查看文件的权限（无法登录后台）的一般职工账户吗？';
      }

      return this.$confirm(msg, '提示', {}).catch(() => { });
    },

    // 编辑提交事件
    editSubmit: function () {
      this.$refs.editForm.validate(valid => {
        if (valid) {
          let params = { personId: this.editModel.Id || 0, username: this.editModel.Username };

          this.toggleLoading();
          personService.userNameExists(params).then(res => {
            this.toggleLoading();

            let { data } = res;
            if (data.exists === true) {
              this.$message('已存在相同的登录账户，不能重复添加');
            } else {
              if (this.isEditing) {
                this.postEdit();
              } else {
                this.postAdd();
              }
            }
          });
        }
      });
    },

    // 添加提交
    postAdd: function () {
      this.submitConfirm().then(() => {
        this.toggleLoading();

        let params = this.editModel;
        personService.add(params).then(res => {
          this.toggleLoading();

          let { code, msg } = res;
          if (code === 201) {
            this.editFormVisible = false;
            this.loadPerson();
            this.$notify({ title: '提示', type: 'success', message: '添加成功' });
          } else {
            this.$message({ type: 'error', message: '添加失败，请稍后重试' });
          }
        }).catch(error => {
          console.info(error);
          this.$message({ type: 'error', message: '添加失败，请稍后重试' });
        });
      });
    },

    // 更新提交
    postEdit: function () {
      this.toggleLoading();

      let params = this.editModel;
      personService.update(params).then(res => {
        this.toggleLoading();

        let { code, msg } = res;
        if (code === 201) {
          this.editFormVisible = false;
          this.loadPerson();
          this.$notify({ title: '提示', type: 'success', message: '编辑成功' });
        } else {
          this.$message({ type: 'error', message: '编辑失败，请稍后重试' });
        }
      }).catch(error => {
        console.info(error);
        this.$message({ type: 'error', message: '编辑失败，请稍后重试' });
      });
    }

  }, // end methods

  mounted() {
    this.loadPerson();
    this.loadDeparts();
  }
};
</script>

<style scoped>
.fr {
  float: right
}

.fl {
  float: left;
}

.desc {
  color: #8492a6;
  font-size: 13px
}

.avatar-uploader .el-upload {
  border: 1px dashed #d9d9d9;
  border-radius: 6px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
}

.avatar-uploader .el-upload:hover {
  border-color: #20a0ff;
}

.avatar-uploader-icon {
  font-size: 28px;
  color: #8c939d;
  width: 178px;
  height: 178px;
  line-height: 178px;
  text-align: center;
}

.avatar {
  width: 178px;
  height: 178px;
  display: block;
}
</style>
