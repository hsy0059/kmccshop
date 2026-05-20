import request from '@/utils/request'

export const loginByPassword = (data: { phone: string; password: string }) =>
  request.post('/v1/auth/password-login', data)

export const getMerchantDashboard = () => request.get('/v1/merchant/dashboard')

export const getProductList = (merchantId: number, params: any) => request.get(`/v1/product/list/${merchantId}`, { params })
export const createProduct = (data: any) => request.post('/v1/product/create', data)
export const updateProduct = (id: number, data: any) => request.put(`/v1/product/${id}`, data)
export const deleteProduct = (id: number) => request.delete(`/v1/product/${id}`)
export const setProductStatus = (id: number, data: any) => request.put(`/v1/product/${id}/status`, data)

export const getMerchantOrders = (params: any) => request.get('/v1/order/merchant/list', { params })
export const acceptOrder = (id: number) => request.post(`/v1/order/${id}/accept`)
export const completeOrder = (id: number) => request.post(`/v1/order/${id}/complete`)
export const cancelOrder = (id: number) => request.post(`/v1/order/${id}/cancel`)

export const getOrderComments = (params: any) => request.get('/v1/order/comment/list', { params })
export const replyComment = (id: number, data: any) => request.post(`/v1/order/comment/${id}/reply`, data)

export const getMerchantCoupons = (params: any) => request.get('/v1/coupon/merchant/list', { params })
export const createCoupon = (data: any) => request.post('/v1/coupon/admin/create', data)
export const updateCoupon = (id: number, data: any) => request.put(`/v1/coupon/admin/${id}`, data)
export const deleteCoupon = (id: number) => request.delete(`/v1/coupon/admin/${id}`)

export const getMerchantInfo = () => request.get('/v1/merchant/my-stats')
export const updateMerchantInfo = (id: number, data: any) => request.put(`/v1/merchant/admin/update/${id}`, data)

export const getMerchantOrderStats = (merchantId: number) => request.get('/v1/order/merchant-stats', { params: { merchantId } })