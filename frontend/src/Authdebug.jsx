import React, { useEffect, useState } from 'react';
import axios from 'axios';

const AuthDebugger = () => {
  const [debugInfo, setDebugInfo] = useState({
    token: null,
    tokenHeader: null,
    decodedToken: null,
    testApiCall: null,
    error: null
  });

  useEffect(() => {
    const runTests = async () => {
      try {
        // 1. Get token from localStorage
        const token = localStorage.getItem('jwtT`oken');
        
        // 2. Setup axios with token
        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        axios.defaults.baseURL = 'http://localhost:5099/api';
        
        // 3. Make a test API call
        let apiResponse = null;
        try {
          const response = await axios.get('/Tasks');
          apiResponse = {
            status: response.status,
            data: response.data
          };
        } catch (error) {
          apiResponse = {
            error: error.message,
            status: error.response?.status,
            data: error.response?.data
          };
        }

        // 4. Decode token (basic decode, not verification)
        let decoded = null;
        if (token) {
          try {
            decoded = JSON.parse(atob(token.split('.')[1]));
          } catch (e) {
            decoded = { error: 'Failed to decode token' };
          }
        }

        // Update state with all debug info
        setDebugInfo({
          token: token,
          tokenHeader: axios.defaults.headers.common['Authorization'],
          decodedToken: decoded,
          testApiCall: apiResponse,
          error: null
        });

      } catch (error) {
        setDebugInfo(prev => ({
          ...prev,
          error: error.message
        }));
      }
    };

    runTests();
  }, []);

  const handleTestRequest = async () => {
    try {
      const response = await axios.get('/Tasks');
      setDebugInfo(prev => ({
        ...prev,
        testApiCall: {
          status: response.status,
          data: response.data
        }
      }));
    } catch (error) {
      setDebugInfo(prev => ({
        ...prev,
        testApiCall: {
          error: error.message,
          status: error.response?.status,
          data: error.response?.data
        }
      }));
    }
  };

  return (
    <div className="p-4 space-y-4">
      <h2 className="text-xl font-bold">Authentication Debug Info</h2>
      
      <div className="space-y-2">
        <h3 className="font-semibold">Token in localStorage:</h3>
        <pre className="bg-gray-100 p-2 rounded overflow-auto">
          {debugInfo.token || 'No token found'}
        </pre>
      </div>

      <div className="space-y-2">
        <h3 className="font-semibold">Authorization Header:</h3>
        <pre className="bg-gray-100 p-2 rounded">
          {debugInfo.tokenHeader || 'No auth header set'}
        </pre>
      </div>

      <div className="space-y-2">
        <h3 className="font-semibold">Decoded Token:</h3>
        <pre className="bg-gray-100 p-2 rounded">
          {JSON.stringify(debugInfo.decodedToken, null, 2)}
        </pre>
      </div>

      <div className="space-y-2">
        <h3 className="font-semibold">Test API Call Result:</h3>
        <button 
          onClick={handleTestRequest}
          className="bg-blue-500 text-white px-4 py-2 rounded mb-2"
        >
          Test API Call
        </button>
        <pre className="bg-gray-100 p-2 rounded">
          {JSON.stringify(debugInfo.testApiCall, null, 2)}
        </pre>
      </div>

      {debugInfo.error && (
        <div className="bg-red-100 border border-red-400 text-red-700 p-2 rounded">
          Error: {debugInfo.error}
        </div>
      )}
    </div>
  );
};

export default AuthDebugger;