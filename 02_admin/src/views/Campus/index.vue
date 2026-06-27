<template>
  <el-card>
    <template #header><div class="card-header"><span>校区管理</span><el-button type="primary" size="small" @click="openCampusDialog()">新增校区</el-button></div></template>

    <el-table :data="campusList" border v-loading="loading" row-key="id" :expand-row-keys="expandedRowKeys">
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="name" label="校区名称" />
      <el-table-column prop="address" label="地址" />
      <el-table-column prop="deliveryRadius" label="配送半径(米)" width="110" />
      <el-table-column label="状态" width="80">
        <template #default="{row}"><el-tag :type="row.status===1?'success':'danger'">{{ row.status===1?'启用':'禁用' }}</el-tag></template>
      </el-table-column>
      <el-table-column label="操作" width="280">
        <template #default="{row}">
          <el-button size="small" @click="openCampusDialog(row)">编辑</el-button>
          <el-button size="small" type="danger" @click="handleDelete(row)">删除</el-button>
          <el-button size="small" type="warning" @click="toggleArea(row)">{{ expandedCampus === row.id ? '收起区域' : '配送区域' }}</el-button>
        </template>
      </el-table-column>

      <el-table-column type="expand" width="1">
        <template #default="{row}">
          <div style="padding:12px 0" v-loading="areaLoading">
            <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:10px">
              <strong>配送区域 & 费用</strong>
              <el-button size="small" type="primary" @click="openAreaDialog(row.id)">新增区域</el-button>
            </div>
            <el-table :data="areaMap[row.id] || []" border size="small">
              <el-table-column prop="name" label="区域名称" />
              <el-table-column prop="deliveryFee" label="配送费(元)" width="100" />
              <el-table-column prop="minOrderAmount" label="起送金额(元)" width="110" />
              <el-table-column prop="estimatedTime" label="预计送达(分钟)" width="120" />
              <el-table-column label="操作" width="160">
                <template #default="{row:z}">
                  <el-button size="small" @click="openAreaDialog(row.id, z)">编辑</el-button>
                  <el-button size="small" type="danger" @click="handleAreaDelete(row.id, z.id)">删除</el-button>
                </template>
              </el-table-column>
            </el-table>
            <el-empty v-if="!areaLoading && (!areaMap[row.id] || areaMap[row.id].length===0)" description="暂无配送区域" :image-size="40" />
          </div>
        </template>
      </el-table-column>
    </el-table>
    <el-empty v-if="!loading && campusList.length===0" description="暂无校区数据" />

    <el-dialog v-model="campusDialog" :title="editCampusId?'编辑校区':'新增校区'" width="500px">
      <el-form :model="campusForm" label-width="100px">
        <el-form-item label="校区名称"><el-input v-model="campusForm.name" /></el-form-item>
        <el-form-item label="地址"><el-input v-model="campusForm.address" /></el-form-item>
        <el-form-item label="配送半径(米)"><el-input-number v-model="campusForm.deliveryRadius" /></el-form-item>
        <el-form-item label="学校ID"><el-input-number v-model="campusForm.schoolId" /></el-form-item>
      </el-form>
      <template #footer><el-button @click="campusDialog=false">取消</el-button><el-button type="primary" @click="saveCampus" :loading="saving">保存</el-button></template>
    </el-dialog>

    <el-dialog v-model="areaDialog" :title="editAreaId?'编辑配送区域':'新增配送区域'" width="500px">
      <el-form :model="areaForm" label-width="130px">
        <el-form-item label="区域名称"><el-input v-model="areaForm.name" /></el-form-item>
        <el-form-item label="配送费(元)"><el-input-number v-model="areaForm.deliveryFee" /></el-form-item>
        <el-form-item label="起送金额(元)"><el-input-number v-model="areaForm.minOrderAmount" /></el-form-item>
        <el-form-item label="预计时间(分钟)"><el-input-number v-model="areaForm.estimatedTime" /></el-form-item>
      </el-form>
      <template #footer><el-button @click="areaDialog=false">取消</el-button><el-button type="primary" @click="saveArea" :loading="saving">保存</el-button></template>
    </el-dialog>
  </el-card>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import {
  getCampusList, createCampus, updateCampus, deleteCampus,
  getDeliveryAreaList, createDeliveryArea, updateDeliveryArea, deleteDeliveryArea
} from '@/api'
import { ElMessage, ElMessageBox } from 'element-plus'

const campusList = ref<any[]>([])
const loading = ref(false)
const expandedCampus = ref(0)
const expandedRowKeys = computed(() => expandedCampus.value ? [expandedCampus.value] : [])
const savedExpand = ref(new Map<number,any[]>())
const areaMap = reactive<Record<number,any[]>>({})
const areaLoading = ref(false)

const fetchList = async () => {
  loading.value = true
  try {
    const res = await getCampusList({})
    campusList.value = res.data || []
  } catch (e) { campusList.value = [] }
  loading.value = false
}
fetchList()

const campusDialog = ref(false)
const editCampusId = ref(0)
const saving = ref(false)
const campusForm = reactive<any>({ name: '', address: '', deliveryRadius: 3000, schoolId: 1 })

const openCampusDialog = (row?: any) => {
  const campus = row || {}
  editCampusId.value = campus.id || 0
  Object.assign(campusForm, {
    name: campus.name || '', address: campus.address || '',
    deliveryRadius: campus.deliveryRadius || 3000, schoolId: campus.schoolId || 1
  })
  campusDialog.value = true
}

const saveCampus = async () => {
  saving.value = true
  try {
    if (editCampusId.value) {
      await updateCampus(editCampusId.value, campusForm)
      ElMessage.success('更新成功')
    } else {
      await createCampus(campusForm)
      ElMessage.success('创建成功')
    }
    campusDialog.value = false
    fetchList()
  } catch (e) { }
  saving.value = false
}

const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm(`确认删除校区"${row.name}"？`, '提示', { type: 'warning' })
    await deleteCampus(row.id)
    ElMessage.success('已删除')
    fetchList()
  } catch (e) { /* cancelled */ }
}

const toggleArea = async (row: any) => {
  if (expandedCampus.value === row.id) {
    expandedCampus.value = 0; return
  }
  expandedCampus.value = row.id
  areaLoading.value = true
  try {
    const res = await getDeliveryAreaList({ campusId: row.id })
    areaMap[row.id] = res.data || []
  } catch (e) { areaMap[row.id] = [] }
  areaLoading.value = false
}

const areaDialog = ref(false)
const editAreaId = ref(0)
const currentCampusId = ref(0)
const areaForm = reactive<any>({ campusId: 0, name: '', deliveryFee: 0, minOrderAmount: 0, estimatedTime: 30 })

const openAreaDialog = (campusId: number, zone?: any) => {
  currentCampusId.value = campusId
  const z = zone || {}
  editAreaId.value = z.id || 0
  Object.assign(areaForm, {
    campusId, name: z.name || '', deliveryFee: z.deliveryFee || 0,
    minOrderAmount: z.minOrderAmount || 0, estimatedTime: z.estimatedTime || 30
  })
  areaDialog.value = true
}

const saveArea = async () => {
  saving.value = true
  try {
    if (editAreaId.value) {
      await updateDeliveryArea(editAreaId.value, areaForm)
      ElMessage.success('更新成功')
    } else {
      await createDeliveryArea(areaForm)
      ElMessage.success('创建成功')
    }
    areaDialog.value = false
    const res = await getDeliveryAreaList({ campusId: areaForm.campusId })
    areaMap[areaForm.campusId] = res.data || []
  } catch (e) { }
  saving.value = false
}

const handleAreaDelete = async (campusId: number, zoneId: number) => {
  try {
    await ElMessageBox.confirm('确认删除此配送区域？', '提示', { type: 'warning' })
    await deleteDeliveryArea(zoneId)
    ElMessage.success('已删除')
    const res = await getDeliveryAreaList({ campusId })
    areaMap[campusId] = res.data || []
  } catch (e) { /* cancelled */ }
}
</script>

<style scoped>
.card-header { display: flex; justify-content: space-between; align-items: center; }
</style>