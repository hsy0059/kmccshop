import request from '@/utils/request'

export const loginByPassword = (data: { phone: string; password: string }) =>
  request.post('/v1/auth/password-login', data)

export const getUserList = (params: any) => request.get('/v1/user/list', { params })
export const updateUser = (id: number, data: any) => request.put(`/v1/user/${id}`, data)
export const deleteUser = (id: number) => request.delete(`/v1/user/${id}`)

export const getMerchantList = (params: any) => request.get('/v1/merchant/list', { params })
export const auditMerchant = (id: number, data: any) => request.post(`/v1/merchant/admin/audit/${id}`, data)
export const updateMerchant = (id: number, data: any) => request.put(`/v1/merchant/admin/update/${id}`, data)

export const getOrderList = (params: any) => request.get('/v1/order/list', { params })
export const getErrandList = (params: any) => request.get('/v1/order/errand/list', { params })

export const getRiderList = (params: any) => request.get('/v1/delivery/rider-admin/list', { params })
export const approveRider = (id: number, data: any) => request.post(`/v1/delivery/rider-admin/approve/${id}`, data)

export const getPostList = (params: any) => request.get('/v1/social/post/list', { params })
export const deletePost = (id: number) => request.delete(`/v1/social/post/delete/${id}`)
export const getSecondGoodsList = (params: any) => request.get('/v1/social/secondhand/list', { params })
export const deleteSecondGoods = (id: number) => request.delete(`/v1/social/secondhand/delete/${id}`)
export const getLostFoundList = (params: any) => request.get('/v1/social/lostandfound/list', { params })
export const deleteLostFound = (id: number) => request.delete(`/v1/social/lostandfound/delete/${id}`)

export const getAdList = (params: any) => request.get('/v1/social/advertisement/list', { params })
export const createAd = (data: any) => request.post('/v1/social/advertisement', data)
export const updateAd = (id: number, data: any) => request.put(`/v1/social/advertisement/${id}`, data)
export const deleteAd = (id: number) => request.delete(`/v1/social/advertisement/${id}`)

export const getCampusList = (params: any) => request.get('/v1/campus/list', { params })
export const createCampus = (data: any) => request.post('/v1/campus', data)
export const updateCampus = (id: number, data: any) => request.put(`/v1/campus/${id}`, data)
export const deleteCampus = (id: number) => request.delete(`/v1/campus/${id}`)

export const getDeliveryAreaList = (params: any) => request.get('/v1/campus/delivery-area/list', { params })
export const createDeliveryArea = (data: any) => request.post('/v1/campus/delivery-area', data)
export const updateDeliveryArea = (id: number, data: any) => request.put(`/v1/campus/delivery-area/${id}`, data)
export const deleteDeliveryArea = (id: number) => request.delete(`/v1/campus/delivery-area/${id}`)

export const getDeliveryFeeList = (params: any) => request.get('/v1/campus/delivery-fee/list', { params })

export const getFeedbackList = (params: any) => request.get('/v1/user/feedback/list', { params })
export const replyFeedback = (id: number, data: any) => request.put(`/v1/user/feedback/${id}/reply`, data)
export const getWithdrawList = (params: any) => request.get('/v1/wallet/withdraws', { params })
export const auditWithdraw = (id: number, data: any) => request.put(`/v1/wallet/withdraw/${id}/audit`, data)

export const getCouponAdminList = (params: any) => request.get('/v1/coupon/admin/list', { params })
export const createCoupon = (data: any) => request.post('/v1/coupon/admin/create', data)
export const updateCoupon = (id: number, data: any) => request.put(`/v1/coupon/admin/${id}`, data)
export const deleteCoupon = (id: number) => request.delete(`/v1/coupon/admin/${id}`)

export const getUserStats = () => request.get('/v1/user/stats')
export const getMerchantStats = () => request.get('/v1/merchant/stats')
export const getOrderStats = () => request.get('/v1/order/statistics')