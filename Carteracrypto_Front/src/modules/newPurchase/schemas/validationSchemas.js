import * as yup from 'yup';

export const schema = yup.object({
  cryptoCode: yup.string().required('You must choose a Cryptocurrency'),
  cryptoAmount: yup.number().required.positive('Amount must be greater than 0'),
  clientId : yup.number().required('Client selection is required'),
  datetime: yup.date().required().max(new Date(), 'Date cannot be in the future')
});
