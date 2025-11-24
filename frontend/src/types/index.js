// User Types
export const UserType = {
  Admin: 1,
  Technician: 2,
  Customer: 3
};

// Ticket Status
export const TicketStatus = {
  New: 1,
  InProgress: 2,
  WaitingCustomer: 3,
  Resolved: 4,
  Closed: 5
};

// Status Labels
export const StatusLabels = {
  [TicketStatus.New]: 'Yeni',
  [TicketStatus.InProgress]: 'İşlemde',
  [TicketStatus.WaitingCustomer]: 'Müşteri Bekliyor',
  [TicketStatus.Resolved]: 'Çözüldü',
  [TicketStatus.Closed]: 'Kapatıldı'
};

// Status Colors
export const StatusColors = {
  [TicketStatus.New]: 'bg-blue-100 text-blue-800',
  [TicketStatus.InProgress]: 'bg-orange-100 text-orange-800',
  [TicketStatus.WaitingCustomer]: 'bg-purple-100 text-purple-800',
  [TicketStatus.Resolved]: 'bg-green-100 text-green-800',
  [TicketStatus.Closed]: 'bg-gray-100 text-gray-800'
};
