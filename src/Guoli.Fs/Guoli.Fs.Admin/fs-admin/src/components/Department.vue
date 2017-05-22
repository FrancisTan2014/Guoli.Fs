<template>
  <section>

    <!--工具条-->
    <el-col :span="24"
            class="toolbar"
            style="padding-bottom: 0px;">
      <el-form :inline="true"
               :model="filters"
               @submit.native.prevent="loadDeparts">
        <el-form-item>
          <el-input v-model="filters.name"
                    placeholder="单位名称"></el-input>
        </el-form-item>
        <el-form-item>
          <el-button type="primary"
                     v-on:click="loadDeparts">查询</el-button>
        </el-form-item>
        <el-form-item>
          <el-button type="primary"
                     @click="handleAdd">新增</el-button>
        </el-form-item>
      </el-form>
    </el-col>

    <!-- 列表 -->
    <el-table :data="departs"
              highlight-current-row
              v-loading="isLoading">
      <el-table-column type="selection"
                       width="55"></el-table-column>
      <el-table-column type="index"
                       width="60"></el-table-column>
      <el-table-column prop="Name"
                       label="单位名称"
                       min-width="200"
                       sortable></el-table-column>
      <el-table-column prop="AddTime"
                       label="添加时间"
                       min-width="200"
                       sortable
                       :formatter="timeFormatter"></el-table-column>
      <el-table-column label="操作"
                       min-width="150">
        <template scope="scope">
          <el-button size="small"
                     @click="handleEdit(scope.$index, scope.row)">修改</el-button>
          <el-button type="danger"
                     size="small"
                     @click="handleDel(scope.$index, scope.row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <!--工具条-->
    <el-col :span="24"
            class="toolbar">
      共
      <span>{{ total }}</span> 条， 每页显示
      <el-select v-model="size"
                 size="small"
                 style="width: 70px;"
                 @change="loadDeparts">
        <el-option v-for="item in sizes"
                   :value="item"
                   :label="item"
                   :key="item"></el-option>
      </el-select>
      条
      <el-pagination layout="prev, pager, next"
                     @current-change="handlePageChange"
                     :page-size="size"
                     :total="total"
                     style="float:right;">
      </el-pagination>
    </el-col>

    <!--新增界面-->
    <el-dialog title="新增"
               v-model="addFormVisible"
               :close-on-click-modal="false">
      <el-form :model="addForm"
               label-width="100px"
               :rules="addFormRules"
               @submit.native.prevent="addSubmit"
               ref="addForm">
        <el-form-item label="单位名称"
                      prop="name">
          <el-input v-model="addForm.name"
                    auto-complete="off"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer"
           class="dialog-footer">
        <el-button @click.native="addFormVisible = false">取消</el-button>
        <el-button type="primary"
                   @click.native="addSubmit"
                   :loading="addLoading">提交</el-button>
      </div>
    </el-dialog>

    <!--编辑界面-->
    <el-dialog title="修改"
               v-model="editFormVisible"
               :close-on-click-modal="false">
      <el-form :model="editForm"
               label-width="100px"
               :rules="editFormRules"
               @submit.native.prevent="editSubmit"
               ref="editForm">
        <el-form-item label="单位名称"
                      prop="name">
          <el-input v-model="editForm.name"
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
import departService from '../services/department-service';
import utils from '../utils';

let rules = {
  name: [
    { required: true, message: '名称不能为空', trigger: 'blur' },
    { max: 20, message: '名称不能超过20个字', trigger: 'blur' }
  ]
};

export default {
  data() {
    return {
      departs: [],
      isLoading: false,
      page: 1,
      sizes: [10, 20, 50, 100],
      size: 10,
      total: 0,
      filters: {
        name: ''
      },

      // 新增表单配置
      addFormVisible: false, // 添加界面的显示与隐藏
      addLoading: false,
      addFormRules: rules,
      addForm: {
        id: 0,
        name: ''
      },

      // 编辑表单配置
      editFormVisible: false,
      editLoading: false,
      editFormRules: rules,
      editForm: {
        name: ''
      }
    };
  }, // end data

  methods: {
    toggleLoading() {
      if (this.addLoading) {
        this.addLoading = false;
        NProgress.done();
      } else {
        this.addLoading = true;
        NProgress.start();
      }
    },

    loadDeparts() {
      this.isLoading = true;
      NProgress.start();

      let params = { page: this.page, size: this.size, conditions: { name: this.filters.name } };
      departService.getPage(params)
        .then(res => {
          this.isLoading = false;
          NProgress.done();

          this.total = res.data.total;
          this.departs = res.data.list;
        });
    },

    // 表格中的时间格式化器
    timeFormatter: (row, col) => {
      return moment(row.AddTime).format('YYYY-MM-DD HH:mm');
    },

    // 新增按钮被点击时触发
    handleAdd: function () {
      this.addFormVisible = true;
      this.addForm = { name: '' };
    },

    // 编辑按钮被点击时触发
    handleEdit: function (index, row) {
      this.editFormVisible = true;
      this.editForm = { id: row.Id, name: row.Name };
    },

    // 删除按钮被点击时触发
    handleDel: function (index, row) {
      let _this = this;
      _this.$confirm(`您确定要删除${row.Name}吗？`, '警告', { type: 'warning' })
        .then(() => {
          _this.toggleLoading();

          let params = { id: row.Id };
          departService.delete(params).then(res => {
            _this.toggleLoading();

            let { code, msg } = res;
            if (code === 204) {
              _this.loadDeparts();
              _this.$notify({
                title: '提示',
                type: 'success',
                message: msg
              });
            } else {
              _this.$notify({
                title: '提示',
                type: 'error',
                message: `${row.Name}包含文件或者人员信息，不能删除！`
              });
            }
          });
        });
    },

    // 分页页码改变时触发
    handlePageChange: function (page) {
      this.page = page;
      this.loadDeparts();
    },

    // 新增单位提交时触发
    addSubmit: function () {
      this.$refs.addForm.validate(valid => {
        if (valid) {
          this.toggleLoading();

          let _this = this;
          let params = { name: _this.addForm.name };
          departService.exists(params)
            .then(res => {
              _this.toggleLoading();

              if (res.data.exists) {
                _this.$message('已存在名称相同的项');
              } else {
                _this.$confirm('确认提交吗？', '提示', {}).then(() => {
                  let params = { Name: _this.addForm.name };
                  departService.add(params).then(res => {
                    let { code, msg } = res;
                    if (code === 201) {
                      _this.addFormVisible = false;
                      _this.$notify({
                        title: '提示',
                        type: 'success',
                        message: '添加成功'
                      });
                      _this.loadDeparts();
                    } else {
                      _this.$notify({
                        title: '提示',
                        type: 'error',
                        message: msg
                      });
                    }
                  });
                });
              }
            })
        }
      });
    },

    // 修改单位提交时触发
    editSubmit: function () {
      this.$refs.editForm.validate(valid => {
        if (valid) {
          let _this = this;
          let params = { name: _this.editForm.name };
          departService.exists(params).then(res => {
            if (res.data.exists) {
              _this.$message('已存在名称相同的项');
            } else {
              _this.editLoading = true;
              NProgress.start();

              let o = _this.editForm;
              let params = { id: o.id, Id: o.id, Name: o.name };
              departService.update(params).then(res => {
                _this.editLoading = false;
                NProgress.done();

                let { code, msg } = res;
                if (code === 201) {
                  _this.editFormVisible = false;
                  _this.loadDeparts();
                  _this.$notify({ title: '提示', type: 'success', message: '修改成功' });
                }
              }).catch(e => {
                _this.editLoading = false;
                NProgress.done();

                _this.$notify({ title: '提示', type: 'error', message: e.toString() });
              });
            }
          });
        }
      });
    }
  }, // end methods

  mounted() {
    this.loadDeparts();
  }
};
</script>

<style scoped>

</style>
