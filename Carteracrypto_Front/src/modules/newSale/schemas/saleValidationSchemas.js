import * as yup from 'yup';

export const saleSchema = yup.object({
  cryptoCode: yup.string().required('Cryptocurrency is required'),
  cryptoAmount: yup.number().typeError('Amount must be a number').required('Amount is required').positive('Amount must be greater than 0'),
  clientId: yup.number().typeError('Client is required').required('Client is required'),
   datetime: yup.date().required().max(new Date(), 'Date and Time is required')
});
