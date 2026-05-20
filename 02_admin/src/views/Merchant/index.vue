<template>
  <el-card>
    <template #header>
      <div class="card-header">
        <span>商家管理</span>
        <el-radio-group v-model="statusFilter" size="small" @change="onFilterChange">
          <el-radio-button :value="undefined">全部</el-radio-button>
          <el-radio-button :value="0">待审核</el-radio-button>
          <el-radio-button :value="1">营业中</el-radio-button>
          <el-radio-button :value="2">休息中</el-radio-button>
          <el-radio-button :value="3">已禁用</el-radio-button>
        </el-radio-group>
      </div>
    </template>

    <el-table :data="list" v-loading="loading" border stripe>
      <el-table-column prop="id" label="ID" width="70" />
      <el-table-column prop="name" label="商家名称" min-width="140" />
      <el-table-column prop="phone" label="电话" width="130" />
      <el-table-column label="评分" width="70">
        <template #default="{ row }">⭐{{ row.rating }}</template>
      </el-table-column>
      <el-table-column prop="monthlySales" label="月销量" width="80" />
      <el-table-column label="状态" width="90">
        <template #default="{ row }">
          <el-tag :type="statusType(row.status)" size="small">{{ statusLabel(row.status) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createdAt" label="入驻时间" width="170" />
      <el-table-column label="操作" width="260" fixed="right">
        <template #default="{ row }">
          <template v-if="row.status === 0">
            <el-button size="small" type="success" @click="handleApprove(row)">通过</el-button>
            <el-button size="small" type="warning" @click="handleReject(row)">拒绝</el-button>
          </template>
          <template v-else-if="row.status === 1">
            <el-button size="small" type="warning" @click="handleAudit(row, 2)">设为休息</el-button>
            <el-button size="small" type="danger" @click="handleAudit(row, 3)">禁用</el-button>
          </template>
          <template v-else-if="row.status === 2">
            <el-button size="small" type="success" @click="handleAudit(row, 1)">设为营业</el-button>
            <el-button size="small" type="danger" @click="handleAudit(row, 3)">禁用</el-button>
          </template>
          <template v-else-if="row.status === 3">
            <el-button size="small" type="primary" @click="handleAudit(row, 1)">恢复营业</el-button>
          </template>
        </template>
      </el-table-column>
    </el-table>

    <div class="pagination-wrap">
      <el-pagination
        v-model:current-page="page"
        :total="total"
        :page-size="20"
        @current-change="fetchData"
        layout="total, prev, pager, next"
        background
      />
    </div>
  </el-card>

  <el-dialog v-model="rejectVisible" title="拒绝商家入驻" width="450px" :close-on-click-modal="false">
    <el-form :model="rejectForm" label-width="80px">
      <el-form-item label="商家名称">
        <el-input :model-value="rejectTarget?.name" disabled />
      </el-form-item>
      <el-form-item label="拒绝原因">
        <el-input v-model="rejectForm.remark" type="textarea" :rows="3" placeholder="请输入拒绝原因（选填）" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="rejectVisible = false">取消</el-button>
      <el-button type="danger" @click="confirmReject" :loading="auditLoading">确认拒绝</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { getMerchantList, auditMerchant } from '@/api'
import { ElMessage, ElMessageBox } from 'element-plus'

interface Merchant {
  id: number
  name: string
  phone: string
  rating: number
  monthlySales: number
  status: number
  createdAt: string
}

const list = ref<Merchant[]>([])
const loading = ref(false)
const auditLoading = ref(false)
const page = ref(1)
const total = ref(0)
const statusFilter = ref<number | undefined>(undefined)

const rejectVisible = ref(false)
const rejectTarget = ref<Merchant | null>(null)
const rejectForm = reactive({ remark: '' })

const statusLabel = (s: number) => ['待审核', '营业中', '休息中', '已禁用'][s] || '未知'
const statusType = (s: number) => ['warning', 'success', 'info', 'danger'][s] || 'info'

const fetchData = async () => {
  loading.value = true
  try {
    const params: any = { page: page.value, pageSize: 20 }
    if (statusFilter.value !== undefined) params.status = statusFilter.value
    const res = await getMerchantList(params)
    list.value = (res.data as any)?.data?.list || (res.data as any)?.list || []
    total.value = (res.data as any)?.data?.total || (res.data as any)?.total || 0
  } finally {
    loading.value = false
  }
}
fetchData()

const onFilterChange = () => {
  page.value = 1
  fetchData()
}

const handleApprove = (row: Merchant) => {
  ElMessageBox.confirm(`确认通过「${row.name}」的入驻申请？`, '审核确认', {
    confirmButtonText: '确认通过',
    cancelButtonText: '取消',
    type: 'success'
  }).then(() => doAudit(row.id, 1))
}

const handleReject = (row: Merchant) => {
  rejectTarget.value = row
  rejectForm.remark = ''
  rejectVisible.value = true
}

const confirmReject = async () => {
  if (!rejectTarget.value) return
  await doAudit(rejectTarget.value.id, 3, rejectForm.remark || undefined)
  rejectVisible.value = false
}

const handleAudit = (row: Merchant, status: number) => {
  const statusText = status === 1 ? '恢复营业' : status === 2 ? '设为休息' : '禁用'
  ElMessageBox.confirm(`确认将「${row.name}」${statusText}？`, '操作确认', {
    confirmButtonText: '确认',
    cancelButtonText: '取消',
    type: status === 3 ? 'error' : 'warning'
  }).then(() => doAudit(row.id, status))
}

const doAudit = async (id: number, status: number, remark?: string) => {
  auditLoading.value = true
  try {
    await auditMerchant(id, { status, remark })
    ElMessage.success('操作成功')
    fetchData()
  } catch {
    ElMessage.error('操作失败')
  } finally {
    auditLoading.value = false
  }
}
</script>

<style scoped lang="scss">
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.pagination-wrap {
  margin-top: 16px;
  text-align: right;
}
</style>