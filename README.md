# DACN_DVTRUCTUYEN
# Một số giá trị được cố định:
switch (Order.PayStatus)
{
    case -2:
        return "Hủy thanh toán";
    case -1:
        return "Thanh toán thất bại";
    case 0:
        return "Chờ thanh toán";
    case 1:
        return "Thanh toán thành công";
    default:
        return "Không thể đọc thông tin";
}

switch (ProductOption.Type)
{
    case 0:
        return "Tạo tài khoản";
    case 1:
        return "Có sẵn";
    case 2:
        return "Đăng ký/mua chính chủ";
    default:
        return "Không thể đọc thông tin";
}
switch (orderstatus_id)
{
    case 1:
        return "Chờ tiến hành";
    case 2:
        return "Đang tiến hành";
    case 3:
        return "Hoàn thành";
    default:
        return "Không thể đọc thông tin";
}
