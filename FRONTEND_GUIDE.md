# 🎨 FRONTEND KULLANIM KILAVUZU

## 📦 Kurulum ve Başlatma

### Backend'i Başlat
```bash
cd "C:\Users\emirr\OneDrive\Masaüstü\TicketSystem\src\TicketSystem.API"
dotnet run
```
**Backend URL:** http://localhost:5000

### Frontend'i Başlat
```bash
cd "C:\Users\emirr\OneDrive\Masaüstü\TicketSystem\frontend"
npm run dev
```
**Frontend URL:** http://localhost:5173

---

## 🎯 Yapılan Özellikler

### ✅ Tamamlanan Sayfalar

#### 1. Authentication
- ✅ Login Page (`/auth/login`)
- ✅ Register Page (`/auth/register`)
- ✅ Auto-redirect (role-based)

#### 2. Customer Panel
- ✅ Customer Dashboard (`/customer`)
  - İstatistikler (toplam, yeni, aktif, çözüldü, kapalı)
  - Hızlı işlemler (Yeni Ticket, Tüm Ticketlar)
  - Son 5 ticket listesi

- ✅ Ticket Oluşturma (`/customer/tickets/new`)
  - Başlık, Kategori, Öncelik, Açıklama
  - Kategori dropdown (API'den çekiliyor)
  - Form validation

- ✅ Ticket Listeleme (`/customer/tickets`)
  - Tüm ticketları görüntüleme
  - Filtreler (Tümü, Aktif, Çözüldü, Kapalı)
  - Status ve Priority badge'leri
  - Responsive tasarım

- ✅ Ticket Detay & Chat (`/customer/tickets/:id`)
  - Ticket bilgileri (durum, öncelik, kategori, tekniker)
  - Real-time Chat (SignalR)
  - Mesaj gönderme
  - Scroll to bottom

#### 3. Admin Panel
- ✅ Admin Dashboard (`/admin`)
  - Toplam istatistikler (tickets, users, technicians)
  - Yönetim paneli linkleri
  - Quick actions

#### 4. Technician Panel
- ✅ Technician Dashboard (`/technician`)
  - Atanan ticket istatistikleri
  - Ticket listesi (kendi kategorisine göre)
  - Ticket detay (Customer ile aynı sayfa kullanıyor)

---

## 🗂️ Klasör Yapısı

```
frontend/src/
├── components/
│   └── layouts/
│       └── Navbar.jsx              # Ana navigasyon
├── context/
│   └── AuthContext.jsx             # Auth state management
├── guards/
│   ├── AuthGuard.jsx               # Login kontrolü
│   ├── RoleGuard.jsx               # Rol bazlı erişim
│   └── GuestGuard.jsx              # Sadece logout kullanıcılar
├── lib/
│   ├── api/
│   │   ├── axios.js                # Axios instance
│   │   ├── auth.api.js             # Auth endpoints
│   │   ├── ticket.api.js           # Ticket endpoints
│   │   ├── user.api.js             # User endpoints
│   │   └── category.api.js         # Category endpoints
│   └── signalr/
│       └── connection.js           # SignalR connection
├── pages/
│   ├── auth/
│   │   ├── LoginPage.jsx
│   │   └── RegisterPage.jsx
│   ├── customer/
│   │   ├── CustomerDashboard.jsx
│   │   ├── CreateTicketPage.jsx
│   │   ├── MyTicketsPage.jsx
│   │   └── TicketDetailPage.jsx
│   ├── admin/
│   │   └── AdminDashboard.jsx
│   └── technician/
│       └── TechnicianDashboard.jsx
├── types/
│   └── index.js                    # Enums ve constants
├── App.jsx                         # Main router
└── index.css                       # Tailwind CSS
```

---

## 🔑 Kullanıcı Rolleri

### UserType Enum
```javascript
UserType.Admin = 1
UserType.Technician = 2
UserType.Customer = 3
```

### Test Kullanıcıları Oluşturma

#### 1. Customer (Otomatik)
- Register sayfasından kayıt ol
- Otomatik `UserType = 3` (Customer) olur

#### 2. Admin (Manuel)
```sql
-- 1. Register sayfasından kayıt ol: admin@example.com
-- 2. MySQL'de çalıştır:
UPDATE Users SET UserType = 1 WHERE Email = 'admin@example.com';
```

#### 3. Technician (Manuel)
```sql
-- 1. Register sayfasından kayıt ol: tech@example.com
-- 2. MySQL'de çalıştır:
UPDATE Users SET UserType = 2 WHERE Email = 'tech@example.com';
```

---

## 🚀 Kullanım Senaryoları

### Senaryo 1: Customer - Yeni Ticket Oluşturma
1. Register: http://localhost:5173/auth/register
2. Login: Otomatik `/customer` dashboard'a yönlendirilir
3. "Yeni Ticket Oluştur" butonuna tıkla
4. Formu doldur (Başlık, Kategori, Öncelik, Açıklama)
5. "Ticket Oluştur" → Otomatik ticket detay sayfasına yönlendirilir
6. Chat ile tekniker ile mesajlaş

### Senaryo 2: Admin - Sistem Yönetimi
1. Kullanıcı oluştur ve MySQL'de Admin yap
2. Login: Otomatik `/admin` dashboard'a yönlendirilir
3. İstatistikleri görüntüle
4. Yönetim paneli linklerine tıkla (Tickets, Users, Technicians, Categories)

### Senaryo 3: Technician - Ticket Yanıtlama
1. Kullanıcı oluştur ve MySQL'de Technician yap
2. Login: Otomatik `/technician` dashboard'a yönlendirilir
3. Atanan ticketları görüntüle
4. Ticket detayına git
5. Chat ile müşteri ile iletişim kur

---

## 📡 SignalR Real-Time Chat

### Özellikler
- ✅ Otomatik bağlantı (token ile)
- ✅ Real-time mesaj gönderme/alma
- ✅ Otomatik reconnect
- ✅ Scroll to bottom

### Hub URL
```
http://localhost:5000/hubs/ticket
```

### Kullanılan Events
```javascript
// Mesaj gönder
connection.invoke('SendMessage', ticketId, message)

// Mesaj al
connection.on('ReceiveMessage', (message) => {})

// Ticket room'a katıl
connection.invoke('JoinTicket', ticketId)
```

---

## 🎨 UI Components & Styling

### Tailwind CSS
- ✅ Responsive design (mobil uyumlu)
- ✅ Dark/Light theme ready
- ✅ Custom colors (primary blue)

### Status Colors
```javascript
New: bg-blue-100 text-blue-800
InProgress: bg-orange-100 text-orange-800
WaitingCustomer: bg-purple-100 text-purple-800
Resolved: bg-green-100 text-green-800
Closed: bg-gray-100 text-gray-800
```

### Priority Colors
```javascript
Low: bg-gray-100 text-gray-800
Medium: bg-blue-100 text-blue-800
High: bg-orange-100 text-orange-800
Urgent: bg-red-100 text-red-800
```

---

## 🔒 Route Guards

### AuthGuard
- Login kontrolü yapar
- Giriş yapmamış kullanıcıları `/auth/login`'e yönlendirir

### RoleGuard
- Rol bazlı erişim kontrolü
- Admin tüm sayfalara erişebilir
- Technician sadece `/technician/*` sayfalarına
- Customer sadece `/customer/*` sayfalarına

### GuestGuard
- Sadece logout kullanıcılar için
- Giriş yapmış kullanıcıları dashboard'larına yönlendirir

---

## 📝 API Endpoints (Backend)

### Auth
```
POST /api/auth/login
POST /api/auth/register
POST /api/auth/refresh-token
```

### Tickets
```
GET    /api/ticket              - Tüm ticketlar
GET    /api/ticket/{id}         - ID'ye göre
GET    /api/ticket/my           - Müşterinin ticketları
GET    /api/ticket/assigned     - Teknisyene atananlar
POST   /api/ticket              - Yeni ticket
PATCH  /api/ticket/{id}/status  - Durum güncelle
POST   /api/ticket/messages     - Mesaj gönder
GET    /api/ticket/{id}/messages - Mesajları getir
```

### Users
```
GET    /api/user                - Tüm kullanıcılar
GET    /api/user/technicians    - Tüm teknisyenler
```

### Categories
```
GET    /api/category            - Tüm kategoriler
```

---

## 🐛 Troubleshooting

### Frontend Hataları

#### 1. "Module not found" hatası
```bash
cd frontend
npm install
```

#### 2. Tailwind CSS yüklenmiyor
```bash
npm install -D tailwindcss@3.4.17 postcss autoprefixer
```

#### 3. SignalR bağlanamıyor
- Backend'in çalıştığından emin ol
- Console'da hata kontrolü yap
- Token'ın localStorage'da olduğunu kontrol et

#### 4. API istekleri başarısız
- Backend URL'i kontrol et: `http://localhost:5000`
- CORS ayarlarını kontrol et
- Token'ın geçerli olduğunu kontrol et

---

## 🎯 Sonraki Adımlar (Opsiyonel Geliştirmeler)

### 1. Admin Panel - Detaylı Sayfalar
- [ ] Tüm Ticketlar Listesi (`/admin/tickets`)
- [ ] Kullanıcı Yönetimi (`/admin/users`)
- [ ] Tekniker Yönetimi (`/admin/technicians`)
- [ ] Kategori Yönetimi (`/admin/categories`)

### 2. Ticket Özellik leri
- [ ] Dosya ekleme (attachment)
- [ ] Ticket öncelik değiştirme
- [ ] Ticket kategori değiştirme
- [ ] Ticket geçmişi görüntüleme

### 3. Bildirimler
- [ ] In-app notifications
- [ ] Toast messages
- [ ] Gerçek zamanlı bildirimler (SignalR)

### 4. Arama ve Filtreleme
- [ ] Ticket numarasıyla arama
- [ ] İleri seviye filtreler
- [ ] Tarih aralığı filtreleme

### 5. Profil ve Ayarlar
- [ ] Kullanıcı profili
- [ ] Şifre değiştirme
- [ ] Bildirim tercihleri

---

## 📚 Kullanılan Teknolojiler

### Frontend
- React 19
- Vite
- React Router v6
- Axios
- @microsoft/signalr
- Tailwind CSS
- date-fns

### Backend
- .NET 8 Web API
- MySQL
- Entity Framework Core
- SignalR
- JWT Authentication

---

## 🎓 Önemli Notlar

1. **Token Yönetimi:**
   - Token localStorage'da saklanıyor
   - Axios interceptor ile otomatik ekleniyor
   - 401 hatası geldiğinde otomatik logout

2. **SignalR Bağlantısı:**
   - Ticket detay sayfası açıldığında otomatik bağlanıyor
   - Sayfa kapatıldığında bağlantı kesiliyor
   - Otomatik reconnect özelliği var

3. **Route Guards:**
   - Admin tüm route'lara erişebilir
   - Roller arası geçiş otomatik redirect ile yönetiliyor

4. **Responsive Tasarım:**
   - Mobil, tablet, desktop uyumlu
   - Tailwind breakpoints kullanılıyor

---

**Oluşturma Tarihi:** 17 Kasım 2025
**Versiyon:** 1.0
**Durum:** Tamamlandı ✅
