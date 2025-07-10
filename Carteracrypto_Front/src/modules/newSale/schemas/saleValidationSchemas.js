import * as yup from 'yup';

export const saleSchema = yup.object({
  cryptoCode: yup.string().required('Cryptocurrency is required'),
  cryptoAmount: yup.number().required('Amount is required').positive('Amount must be greater than 0'),
  clientId: yup.number().required('Client is required'),
  datetime: yup.string().required('Date and Time is required')
});
