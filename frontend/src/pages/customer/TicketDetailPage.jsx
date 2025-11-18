import { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ticketAPI } from '../../lib/api/ticket.api';
import { startConnection, stopConnection, getConnection } from '../../lib/signalr/connection';
import { useAuth } from '../../context/AuthContext';
import { StatusLabels, StatusColors, PriorityLabels, PriorityColors } from '../../types';
import { format } from 'date-fns';
import { tr } from 'date-fns/locale';

const TicketDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [ticket, setTicket] = useState(null);
  const [messages, setMessages] = useState([]);
  const [newMessage, setNewMessage] = useState('');
  const [loading, setLoading] = useState(true);
  const [sending, setSending] = useState(false);
  const messagesEndRef = useRef(null);
  const [connection, setConnection] = useState(null);

  useEffect(() => {
    fetchTicketDetails();
    setupSignalR();

    return () => {
      if (connection) {
        stopConnection();
      }
    };
  }, [id]);

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const setupSignalR = async () => {
    try {
      const conn = await startConnection();
      setConnection(conn);

      // Listen for new messages
      conn.on('ReceiveMessage', (message) => {
        setMessages(prev => [...prev, message]);
      });

      // Join ticket room
      await conn.invoke('JoinTicket', parseInt(id));
    } catch (error) {
      console.error('SignalR connection error:', error);
    }
  };

  const fetchTicketDetails = async () => {
    try {
      const [ticketData, messagesData] = await Promise.all([
        ticketAPI.getTicketById(id),
        ticketAPI.getTicketMessages(id)
      ]);
      setTicket(ticketData);
      setMessages(messagesData);
    } catch (error) {
      console.error('Error fetching ticket details:', error);
    } finally {
      setLoading(false);
    }
  };

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const handleSendMessage = async (e) => {
    e.preventDefault();
    if (!newMessage.trim() || sending) return;

    setSending(true);
    try {
      // Send message via API - Backend will broadcast via SignalR
      await ticketAPI.sendMessage(parseInt(id), newMessage.trim());
      setNewMessage('');
    } catch (error) {
      console.error('Error sending message:', error);
      alert('Mesaj gönderilemedi');
    } finally {
      setSending(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500 mx-auto"></div>
          <p className="mt-4 text-gray-600">Yükleniyor...</p>
        </div>
      </div>
    );
  }

  if (!ticket) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <p className="text-gray-600">Ticket bulunamadı</p>
          <button
            onClick={() => navigate('/customer/tickets')}
            className="mt-4 text-blue-600 hover:text-blue-700"
          >
            Geri Dön
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="mb-6">
          <button
            onClick={() => navigate('/customer/tickets')}
            className="flex items-center text-gray-600 hover:text-gray-900 mb-4"
          >
            <svg className="w-5 h-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
            Tüm Ticketlar
          </button>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Ticket Info */}
          <div className="lg:col-span-1">
            <div className="bg-white shadow rounded-lg p-6">
              <h2 className="text-lg font-semibold text-gray-900 mb-4">Ticket Bilgileri</h2>

              <div className="space-y-4">
                <div>
                  <p className="text-sm text-gray-500">Ticket Numarası</p>
                  <p className="text-base font-semibold text-gray-900">#{ticket.ticketNumber}</p>
                </div>

                <div>
                  <p className="text-sm text-gray-500">Durum</p>
                  <span className={`inline-block px-2 py-1 text-xs font-medium rounded ${StatusColors[ticket.status]}`}>
                    {StatusLabels[ticket.status]}
                  </span>
                </div>

                <div>
                  <p className="text-sm text-gray-500">Öncelik</p>
                  <span className={`inline-block px-2 py-1 text-xs font-medium rounded ${PriorityColors[ticket.priority]}`}>
                    {PriorityLabels[ticket.priority]}
                  </span>
                </div>

                <div>
                  <p className="text-sm text-gray-500">Kategori</p>
                  <p className="text-base text-gray-900">{ticket.categoryName}</p>
                </div>

                <div>
                  <p className="text-sm text-gray-500">Atanan Tekniker</p>
                  <p className="text-base text-gray-900">
                    {ticket.assignedTechnicianName || 'Henüz atanmadı'}
                  </p>
                </div>

                <div>
                  <p className="text-sm text-gray-500">Oluşturulma Tarihi</p>
                  <p className="text-base text-gray-900">
                    {format(new Date(ticket.createdAt), 'dd MMMM yyyy HH:mm', { locale: tr })}
                  </p>
                </div>
              </div>
            </div>

            <div className="bg-white shadow rounded-lg p-6 mt-6">
              <h2 className="text-lg font-semibold text-gray-900 mb-4">Açıklama</h2>
              <p className="text-sm text-gray-700 whitespace-pre-wrap">{ticket.description}</p>
            </div>
          </div>

          {/* Chat */}
          <div className="lg:col-span-2">
            <div className="bg-white shadow rounded-lg flex flex-col" style={{ height: 'calc(100vh - 200px)' }}>
              <div className="p-4 border-b border-gray-200">
                <h2 className="text-lg font-semibold text-gray-900">{ticket.title}</h2>
                <p className="text-sm text-gray-500">Tekniker ile mesajlaşın</p>
              </div>

              {/* Messages */}
              <div className="flex-1 overflow-y-auto p-4 space-y-4">
                {messages.length === 0 ? (
                  <div className="text-center py-12">
                    <svg
                      className="mx-auto h-12 w-12 text-gray-400"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"
                      />
                    </svg>
                    <p className="mt-2 text-sm text-gray-500">Henüz mesaj yok</p>
                    <p className="text-xs text-gray-400">İlk mesajı göndererek konuşmayı başlatın</p>
                  </div>
                ) : (
                  messages.map((message) => {
                    const isOwnMessage = message.senderId === user.userId;
                    return (
                      <div
                        key={message.id}
                        className={`flex ${isOwnMessage ? 'justify-end' : 'justify-start'}`}
                      >
                        <div className={`max-w-xs lg:max-w-md ${isOwnMessage ? 'order-2' : 'order-1'}`}>
                          <div
                            className={`rounded-lg px-4 py-2 ${
                              isOwnMessage
                                ? 'bg-blue-600 text-white'
                                : 'bg-gray-100 text-gray-900'
                            }`}
                          >
                            <p className="text-sm whitespace-pre-wrap">{message.message}</p>
                          </div>
                          <p className={`mt-1 text-xs text-gray-500 ${isOwnMessage ? 'text-right' : 'text-left'}`}>
                            {message.senderName} • {format(new Date(message.sentAt), 'HH:mm', { locale: tr })}
                          </p>
                        </div>
                      </div>
                    );
                  })
                )}
                <div ref={messagesEndRef} />
              </div>

              {/* Message Input */}
              <div className="p-4 border-t border-gray-200">
                <form onSubmit={handleSendMessage} className="flex space-x-2">
                  <input
                    type="text"
                    value={newMessage}
                    onChange={(e) => setNewMessage(e.target.value)}
                    placeholder="Mesajınızı yazın..."
                    className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    disabled={sending}
                  />
                  <button
                    type="submit"
                    disabled={!newMessage.trim() || sending}
                    className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    {sending ? (
                      <svg className="animate-spin h-5 w-5" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                      </svg>
                    ) : (
                      <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
                      </svg>
                    )}
                  </button>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TicketDetailPage;
